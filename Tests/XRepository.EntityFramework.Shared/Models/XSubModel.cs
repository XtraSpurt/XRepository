using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XRepository.EntityFramework.Shared
{
    public class XSubModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string SubName { get; set; }

        public Guid ModelId { get; set; }

        public XModel Model { get; set; } 
    }
}
