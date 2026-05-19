using Azure.Storage.Blobs.Models;
using Lunamaroapi.Models.CategoryEnums;

namespace Lunamaroapi.Models
{
    public class CategoryRelationship
    {
        public int Id { get; set; }
        public Priorty Priorty { get; set; }
        public RelationType Type { get; set; }
        public  int CategoryId{get;set;}
        public int RelatedCategoryId { get; set; }


        public Category Category { get; set; } = null!;
        public Category RelatedCategory { get; set; } = null!;
    }
}
