using System.ComponentModel.DataAnnotations;

namespace TP.Upgrade.Domain.Models.DBEntity
{
    public class Category : BaseEntity<short>
    {
        [Required, MaxLength(40)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string? Description { get; set; } = string.Empty;
        public int? CategoryTemplateId { get; set; } = 0;
        [MaxLength(80)]
        public string? MetaKeywords { get; set; } = string.Empty;
        [MaxLength(80)]
        public string? MetaTitle { get; set; } = string.Empty;
        [Required]
        public int ParentCategoryId { get; set; }
        [Required]
        public int ChildLevel { get; set; }
        [Required,MaxLength(100)]
        public string ImageURL { get; set; }
        [Required]
        public bool ShowOnHomepage { get; set; }
        [Required]
        public bool IncludeInTopMenu { get; set; }
        [Required]
        public bool LimitedToStores { get; set; }
        [Required]
        public bool Published { get; set; }
        public bool IsPosterActive { get; set; }
        [Required]
        public int DisplayOrder { get; set; }
        public bool IsDeleted { get; set; }
    }
}
