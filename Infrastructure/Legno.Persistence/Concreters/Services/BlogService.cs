using AutoMapper;
using Legno.Application.Absrtacts.Services;
using Legno.Application.Abstracts.Repositories.Blogs;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Blog;
using Legno.Application.GlobalExceptionn;          // lazım olsa öz namespace-inlə dəyiş
using Legno.Domain.Entities;
using Legno.Domain.HelperEntities;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Legno.Persistence.Concreters.Services
{
    public class BlogService : IBlogService
    {
        private readonly IBlogReadRepository _blogReadRepository;
        private readonly IBlogWriteRepository _blogWriteRepository;
        private readonly ISubscriberService _subscriberService;
        private readonly IMailService _mailService;
        private readonly IFileService _fileService;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;

        public BlogService(
            IBlogReadRepository blogReadRepository,
            IBlogWriteRepository blogWriteRepository,
            ISubscriberService subscriberService,
            IMailService mailService,
            IFileService fileService,
            IMapper mapper,
            CloudinaryService cloudinaryService)
        {
            _blogReadRepository = blogReadRepository;
            _blogWriteRepository = blogWriteRepository;
            _subscriberService = subscriberService;
            _mailService = mailService;
            _fileService = fileService;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<BlogDto> AddBlogAsync(CreateBlogDto dto)
        {
            // Sadə limit nümunələri (istəsən artır)
            if (!string.IsNullOrWhiteSpace(dto.Title) && dto.Title.Length > 200)
                throw new GlobalAppException("Başlıq maksimum 200 simvol ola bilər.");

            var blog = _mapper.Map<Blog>(dto);
            blog.CreatedDate = DateTime.UtcNow;
            blog.LastUpdatedDate = blog.CreatedDate;
            blog.IsDeleted = false;

            // Şəkillər
            if (dto.BlogImage != null)
                blog.BlogImage = await _cloudinaryService.UploadFileAsync(dto.BlogImage);
            if (dto.AuthorImage != null)
                blog.AuthorImage = await _cloudinaryService.UploadFileAsync(dto.AuthorImage);

            await _blogWriteRepository.AddAsync(blog);
            await _blogWriteRepository.CommitAsync();

            // Subscriber-lərə mail
            var allEmails = await _subscriberService.GetAllSubscribersAsync(); // List<string>
            if (allEmails != null && allEmails.Any())
            {
                // DTO formatlanmış CreatedDate-i string verir (Profile sayəsində)
                var blogDto = _mapper.Map<BlogDto>(blog); // CreatedDate: dd.MM.yyyy

                var subject = $"Yeni blog: {blog.Title}";
                foreach (var email in allEmails)
                {
                    var body = BuildBlogEmailBody(blogDto); // HTML şablon (aşağıda)
                    var mailRequest = new MailRequest
                    {
                        ToEmail = email,
                        Subject = "Yeni Blog Var",
                        Body = body
                    };
                    try
                    {
                        await _mailService.SendFromInfoAsync(mailRequest);
                    }
                    catch
                    {
                        throw new GlobalAppException("Mail gonderilerken xeta bas verdi");
                    }
                }
            }

            return _mapper.Map<BlogDto>(blog);
        }

        public async Task<BlogDto?> GetBlogAsync(string blogId)
        {
            var blog = await _blogReadRepository.GetByIdAsync(blogId, EnableTraking: false);
            if (blog == null || blog.IsDeleted) return null;
            return _mapper.Map<BlogDto>(blog);
        }

        public async Task<List<BlogDto>> GetAllBlogsAsync()
        {
            var blogs = await _blogReadRepository.GetAllAsync(
                func: b => !b.IsDeleted,
                include: null,
                orderBy: q => q.OrderByDescending(b => b.CreatedDate), // ən son əvvəl
                EnableTraking: false
            );
            return _mapper.Map<List<BlogDto>>(blogs.ToList());
        }

        public async Task<BlogDto> UpdateBlogAsync(UpdateBlogDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Id))
                throw new GlobalAppException("Id boş ola bilməz.");

            var blog = await _blogReadRepository.GetByIdAsync(dto.Id, EnableTraking: true);
            if (blog == null || blog.IsDeleted)
                throw new GlobalAppException("Blog tapılmadı!");

            // Manual update: null gələnlər toxunulmur
            if (dto.Title != null) blog.Title = dto.Title;
            if (dto.TitleEng != null) blog.TitleEng = dto.TitleEng;
            if (dto.TitleRu != null) blog.TitleRu = dto.TitleRu;
            if (dto.SubTitle != null) blog.SubTitle = dto.SubTitle;
            if (dto.SubTitleEng != null) blog.SubTitleEng = dto.SubTitleEng;
            if (dto.SubTitleRu != null) blog.SubTitleRu = dto.SubTitleRu;
            if (dto.AuthorName != null) blog.AuthorName = dto.AuthorName;
            if (dto.AuthorNameEng != null) blog.AuthorNameEng = dto.AuthorNameEng;
            if (dto.AuthorNameRu != null) blog.AuthorNameRu = dto.AuthorNameRu;
            if (dto.AuthorProfession != null) blog.AuthorProfession = dto.AuthorProfession;


            // Şəkil dəyişimi
            if (dto.BlogImage != null)
            {
                if (!string.IsNullOrEmpty(blog.BlogImage))
                    await _cloudinaryService.DeleteFileAsync(blog.BlogImage);
                blog.BlogImage = await _cloudinaryService.UploadFileAsync(dto.BlogImage);
            }
            if (dto.AuthorImage != null)
            {
                if (!string.IsNullOrEmpty(blog.AuthorImage))
                    await _cloudinaryService.DeleteFileAsync(blog.AuthorImage);
                blog.AuthorImage = await _cloudinaryService.UploadFileAsync(dto.AuthorImage);
            }

            blog.LastUpdatedDate = DateTime.UtcNow;

            await _blogWriteRepository.UpdateAsync(blog);
            await _blogWriteRepository.CommitAsync();

            return _mapper.Map<BlogDto>(blog);
        }

        public async Task DeleteBlogAsync(string blogId)
        {
            var blog = await _blogReadRepository.GetByIdAsync(blogId, EnableTraking: true);
            if (blog == null || blog.IsDeleted)
                throw new GlobalAppException("Blog tapılmadı!");

            blog.IsDeleted = true;
            blog.DeletedDate = DateTime.UtcNow;
            blog.LastUpdatedDate = DateTime.UtcNow;

            await _blogWriteRepository.UpdateAsync(blog);
            await _blogWriteRepository.CommitAsync();
        }

private static string BuildBlogEmailBody(BlogDto blogDto, string? baseUrl = null)
    {
        if (blogDto == null) throw new GlobalAppException(nameof(blogDto));

        // DTO-dan dəyərləri təhlükəsizləşdirək
        var title = WebUtility.HtmlEncode(blogDto.Title ?? "");
        var author = WebUtility.HtmlEncode(blogDto.AuthorName ?? "");
        var dateStr = string.IsNullOrWhiteSpace(blogDto.CreatedDate)
                        ? DateTime.UtcNow.ToString("dd.MM.yyyy")
                        : blogDto.CreatedDate;

        // URL hazırda: /blog/{id} (sonradan baseUrl verə bilərsən)
        var blogUrlRaw = (string.IsNullOrWhiteSpace(baseUrl) ? "" : baseUrl.TrimEnd('/')) + $"/blog/{blogDto.Id}";
        var blogUrl = WebUtility.HtmlEncode(blogUrlRaw);

        string emailBody = $@"
<!doctype html>
<html lang='az'>
<head>
  <meta charset='UTF-8'>
  <title>Yeni Blog</title>
  <style>
    body {{
      font-family: Arial, sans-serif;
      background-color: #f8f9fa;
      margin: 0; padding: 0;
    }}
    .email-container {{
      max-width: 600px;
      margin: 40px auto;
      background-color: #ffffff;
      border: 1px solid #ddd;
      border-radius: 8px;
      overflow: hidden;
    }}
    .header {{
      background-color: #f0f0f0;
      padding: 20px;
      text-align: center;
    }}
    .header img {{ height: 60px; }}
    .content {{
      padding: 30px;
      text-align: left;
      color: #333;
    }}
    .content h2 {{ text-align:center; margin-top: 0; }}
    .info {{ margin: 12px 0; font-size: 16px; color: #555; }}
    .info strong {{ color: #000; }}
    .cta {{ text-align:center; margin-top: 24px; }}
    .btn {{
      display:inline-block; padding:12px 20px; background:#0D60FE; color:#fff;
      border-radius:6px; text-decoration:none;
    }}
    .footer {{
      padding: 16px; text-align: center; background-color: #f0f0f0;
      font-size: 14px; color: #777;
    }}
    a {{ color:#0D60FE; text-decoration:none; }}
  </style>
</head>
<body>
  <div class='email-container'>
    <div class='header'>
      <img src='https://i.postimg.cc/XXMpxN2z/Frame-290.png alt='Logo'>
    </div>

    <div class='content'>
      <h2>Yeni blogunuz var</h2>
      <div class='info'><strong>Blogun adı:</strong> {title}</div>
      <div class='info'><strong>Yaradıcı:</strong> {author}</div>
      <div class='info'><strong>Yaranma tarixi:</strong> {dateStr}</div>

      <div class='cta'>
        <a class='btn' href='{blogUrl}'>Bloqa keçid</a>
      </div>
    </div>

    <div class='footer'>
      Bu mesaj Legno tərəfindən avtomatik göndərilmişdir.
    </div>
  </div>
</body>
</html>
";

        return emailBody;
    }


}
}
