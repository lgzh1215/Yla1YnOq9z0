using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spyder.Core{
    class D2Pass {
    }

    class Item {
        public string image_url { get; set; }
        public List<Review> reviews { get; set; }
        public string total_count { get; set; }
        public Item () {
            reviews = new List<Review>();
        }
    }

    class Review {
        public float avg_rating { get; set; }
        public int bookmark_count { get; set; }
        public int count { get; set; }
        public DateTime created { get; set; } // 2012-11-06 07:05:36.82416+00
        public int has_thumbnail { get; set; } // 0
        public string private_profile { get; set; } // 0
        public long product_id { get; set; } // 4261
        public string product_type { get; set; } // movies
        public int product_type_id { get; set; } // 1
        public string product_url { get; set; }
        public long site_id { get; set; }
        public string site_name { get; set; }
        public string site_name_short { get; set; }
        public string title { get; set; }
        public string title_short { get; set; }
        public List<SubReview> reviews { get; set; }
        public Review () {
            reviews = new List<SubReview>();
        }
    }

    class SubReview {
        public string created { get; set; }
        public string created_jp { get; set; }
        public string like_count { get; set; }
        public string private_profile { get; set; } // 0
        public string product_id { get; set; }
        public string product_type_id { get; set; }
        public string profile_id { get; set; }
        public string profile_name { get; set; }
        public string review_id { get; set; }
        public string sample_url { get; set; }
        public string site_id { get; set; }
        public string status { get; set; }
        public string title { get; set; }
        public string title_short { get; set; }
        public string updated { get; set; }
        public string user_comment { get; set; }
        public string user_id { get; set; }
        public string user_rating { get; set; }
        public SubReview () { }
    }
}
