// Legno.Persistence/Concreters/Services/InMemoryFuzzyProjectSearchService.cs
using AutoMapper;
using Legno.Application.Abstracts.Repositories.Categories;
using Legno.Application.Abstracts.Repositories.Projects;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Project;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Persistence.Concreters.Services
{
    public class InMemoryFuzzyProjectSearchService : IFuzzyProjectSearchService
    {
        private readonly IProjectReadRepository _projectRead;
        private readonly ICategoryReadRepository _categoryRead;
        private readonly IMapper _mapper;

        // Weights (trigram vs levenshtein) – fallback üçün
        private const double WTrigram = 0.7;
        private const double WLevenshtein = 0.3;

        public InMemoryFuzzyProjectSearchService(
            IProjectReadRepository projectRead,
            ICategoryReadRepository categoryRead,
            IMapper mapper)
        {
            _projectRead = projectRead;
            _categoryRead = categoryRead;
            _mapper = mapper;
        }

        public async Task<List<ProjectDto>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new GlobalAppException("Axtarış sorğusu boş ola bilməz.");

            var q = Normalize(query);
            var (catTh, projTh) = GetThresholds(q);

            var categories = await _categoryRead.GetAllAsync(x => !x.IsDeleted, EnableTraking: false);

            var projects = await _projectRead.GetAllAsync(
                func: p => !p.IsDeleted,
                include: x => x.Include(p => p.Category),
                orderBy: null,
                EnableTraking: false
            );

            var bestCategory = categories
                .Select(c => new
                {
                    Cat = c,
                    Score = MaxScoreAcrossFields(q, c.Name, c.NameEng, c.NameRu)
                })
                .OrderByDescending(x => x.Score)
                .FirstOrDefault();

            IEnumerable<(Project Project, double Score)> ranked;

            if (bestCategory != null && bestCategory.Score >= catTh)
            {
                // 2A) Yüksək uyğunluq — həmin kateqoriyanın layihələrində axtar
                ranked = projects
                    .Where(p => p.CategoryId == bestCategory.Cat.Id)
                    .Select(p => (Project: p, Score: MaxScoreAcrossFields(q, p.Title, p.TitleEng, p.TitleRu)))
                    .Where(x => x.Score >= projTh)
                    .OrderByDescending(x => x.Score)
                    .ThenBy(x => x.Project.Title ?? x.Project.TitleEng ?? x.Project.TitleRu);
            }
            else
            {
                // 2B) Kateqoriya yetərli deyil — bütün layihələrdə fuzzy axtar
                ranked = projects
                    .Select(p => (Project: p, Score: MaxScoreAcrossFields(
                        q,
                        p.Title, p.TitleEng, p.TitleRu,
                        p.Category?.Name, p.Category?.NameEng, p.Category?.NameRu)))
                    .Where(x => x.Score >= projTh)
                    .OrderByDescending(x => x.Score)
                    .ThenBy(x => x.Project.Title ?? x.Project.TitleEng ?? x.Project.TitleRu);
            }

            var resultEntities = ranked
             
                .Select(x => x.Project)
                .ToList();

            return _mapper.Map<List<ProjectDto>>(resultEntities);
        }

        // ==== Thresholdları sorğunun uzunluğuna görə tənzimlə ====
        private static (double CategoryThreshold, double ProjectThreshold) GetThresholds(string q)
        {
            // 3 və daha az simvol üçün eşikləri salırıq
            if (q.Length <= 3) return (0.45, 0.40);
            if (q.Length <= 5) return (0.60, 0.50);
            return (0.78, 0.52);
        }

        // ---- Scoring helpers ----
        private double MaxScoreAcrossFields(string q, params string?[] fields)
        {
            double best = 0;
            foreach (var f in fields)
            {
                if (string.IsNullOrWhiteSpace(f)) continue;
                var s = FieldScore(q, Normalize(f));
                if (s > best) best = s;
                if (best >= 0.999) break; // early-exit
            }
            return best;
        }

        /// <summary>
        /// Qısa sorğular üçün prefix/contains boost + substring pəncərəsi,
        /// sonra CombinedScore (trigram+levenshtein) fallback.
        /// </summary>
        private double FieldScore(string query, string field)
        {
            if (string.IsNullOrWhiteSpace(field)) return 0;
            var q = query;         // artıq Normalize olunub
            var t = field;         // Normalize(f) ilə gəlir

            // 1) Prefix / Contains boost – “str” -> “string”
            if (t.StartsWith(q)) return 1.0;
            if (t.Contains(q)) return 0.92;

            // 2) Qısa sorğular üçün substring pəncərəsi (ən yaxşı uyğunluq)
            double best = 0;
            if (t.Length >= q.Length)
            {
                for (int i = 0; i <= t.Length - q.Length; i++)
                {
                    var sub = t.Substring(i, q.Length);
                    best = Math.Max(best, LevenshteinSimilarity(q, sub));
                    if (best >= 0.95) break;
                }
            }

            // 3) Tam mətndə Combined fallback (trigram + levenshtein)
            best = Math.Max(best, CombinedScore(q, t));
            return best;
        }

        private double CombinedScore(string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return 0;

            var trig = TrigramJaccard(a, b);
            var lev = LevenshteinSimilarity(a, b);
            return (trig * WTrigram) + (lev * WLevenshtein);
        }

        // Normalize: lower + diacritics remove + AZ chars mapping
        private static string Normalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            var lower = input.Trim().ToLowerInvariant();

            // AZ/TR spesifik
            lower = lower
                .Replace('ə', 'e')
                .Replace('ğ', 'g')
                .Replace('ı', 'i')
                .Replace('ö', 'o')
                .Replace('ş', 's')
                .Replace('ü', 'u')
                .Replace('ç', 'c');

            // diakritikləri sil
            var formD = lower.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(capacity: formD.Length);
            foreach (var ch in formD)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        // Trigram set
        private static HashSet<string> Trigrams(string s)
        {
            var padded = "  " + s + "  "; // kənarlar üçün pad
            var set = new HashSet<string>();
            for (int i = 0; i < padded.Length - 2; i++)
                set.Add(padded.Substring(i, 3));
            return set;
        }

        private static double TrigramJaccard(string a, string b)
        {
            var A = Trigrams(a);
            var B = Trigrams(b);
            if (A.Count == 0 && B.Count == 0) return 1;
            if (A.Count == 0 || B.Count == 0) return 0;

            int inter = A.Intersect(B).Count();
            int union = A.Count + B.Count - inter;
            return union == 0 ? 0 : (double)inter / union;
        }

        // Levenshtein similarity: 1 - (distance / maxLen)
        private static double LevenshteinSimilarity(string a, string b)
        {
            int d = LevenshteinDistance(a, b);
            int m = Math.Max(a.Length, b.Length);
            if (m == 0) return 1;
            return 1.0 - ((double)d / m);
        }

        private static int LevenshteinDistance(string s, string t)
        {
            var n = s.Length; var m = t.Length;
            if (n == 0) return m;
            if (m == 0) return n;

            var dp = new int[n + 1, m + 1];
            for (int i = 0; i <= n; i++) dp[i, 0] = i;
            for (int j = 0; j <= m; j++) dp[0, j] = j;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                    dp[i, j] = Math.Min(
                        Math.Min(dp[i - 1, j] + 1,     // deletion
                                 dp[i, j - 1] + 1),    // insertion
                        dp[i - 1, j - 1] + cost       // substitution
                    );
                }
            }
            return dp[n, m];
        }
    }
}
