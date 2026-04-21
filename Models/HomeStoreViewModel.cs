using WebStore.Models.Entities;

namespace WebStore.Models
{
    public class HomeStoreViewModel
    {
        public List<ProductViewModel> ProductOutstanding { get; set; }          
        public List<ProductViewModel> ProductBestSelling { get; set; }          
    }
}
