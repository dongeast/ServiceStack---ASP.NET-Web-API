
namespace ProductsApp.Models
{
    public class Response
    {
        public int Result { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// 推广包
    /// </summary>
    public class Bean
    {
        /// <summary>
        /// 推广包
        /// </summary>
        public Bean() { }

        /// <summary>
        /// 推广包
        /// </summary>
        /// <param name="beanId">包名</param>
        /// <param name="rank">优先级</param>
        /// <param name="area">区域</param>
        public Bean(string beanId, int rank, string area)
        {
            this.b = beanId;
            this.r = rank;
            this.a = area;
        }

        /// <summary>
        /// 包名 BeanId
        /// </summary>
        public string b { get; set; }

        /// <summary>
        /// 优先级 Rank
        /// </summary>
        public int r { get; set; }

        /// <summary>
        /// 区域 Area
        /// </summary>
        public string a { get; set; }
    }
}