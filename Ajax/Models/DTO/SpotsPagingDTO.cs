namespace Ajax.Models.DTO
{
    public class SpotsPagingDTO
    {
        public int TotalPages { get; set; } //當後端有計算好搜尋完畢總共有幾頁時
        public List<SpotImagesSpot>? SpotsResult { get; set; }
    }
}
