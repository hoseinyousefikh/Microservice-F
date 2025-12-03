//using Elasticsearch.Net;
//using Nest;
//using System.Collections.Generic;
//using System.Linq;

//namespace Catalog_Service.src._02_Infrastructure.Search
//{
//    public static class SearchQueryBuilder
//    {
//        public static QueryContainer BuildQuery(ProductSearchRequest request)
//        {
//            var container = new QueryContainer();

//            // Full-text search
//            if (!string.IsNullOrWhiteSpace(request.Query))
//            {
//                container &= new QueryContainerDescriptor<ProductDocument>()
//                    .MultiMatch(m => m
//                        .Fields(f => f
//                            .Field(p => p.Name, 2.0)
//                            .Field(p => p.Description)
//                            .Field(p => p.CategoryName)
//                            .Field(p => p.BrandName))
//                        .Query(request.Query)
//                        .Fuzziness(Fuzziness.Auto)
//                        .Operator(Operator.Or));
//            }

//            // Category filter
//            if (request.CategoryIds?.Any() == true)
//            {
//                container &= new QueryContainerDescriptor<ProductDocument>()
//                    .Terms(t => t
//                        .Field(f => f.CategoryId)
//                        .Terms(request.CategoryIds.Select(id => (object)id).ToArray()));
//            }

//            // Brand filter
//            if (request.BrandIds?.Any() == true)
//            {
//                container &= new QueryContainerDescriptor<ProductDocument>()
//                    .Terms(t => t
//                        .Field(f => f.BrandId)
//                        .Terms(request.BrandIds.Select(id => (object)id).ToArray()));
//            }

//            // Price range filter
//            if (request.MinPrice.HasValue || request.MaxPrice.HasValue)
//            {
//                container &= new QueryContainerDescriptor<ProductDocument>()
//                    .Range(r => r
//                        .Field(f => f.Price)
//                        .GreaterThanOrEquals(request.MinPrice.HasValue ? (double?)request.MinPrice.Value : null)
//                        .LessThanOrEquals(request.MaxPrice.HasValue ? (double?)request.MaxPrice.Value : null));
//            }

//            // In stock filter
//            if (request.InStockOnly)
//            {
//                container &= new QueryContainerDescriptor<ProductDocument>()
//                    .Term(t => t
//                        .Field(f => f.InStock)
//                        .Value(true));
//            }

//            // Active products only
//            container &= new QueryContainerDescriptor<ProductDocument>()
//                .Term(t => t
//                    .Field(f => f.IsActive)
//                    .Value(true));

//            return container;
//        }

//        // روش 1: برگرداندن Func<SortDescriptor<ProductDocument>, IPromise<IList<ISort>>>
//        // این روش بهترین و رایج‌ترین راه در NEST است
//        public static Func<SortDescriptor<ProductDocument>, IPromise<IList<ISort>>> BuildSort(ProductSearchRequest request)
//        {
//            return sort => {
//                if (string.IsNullOrWhiteSpace(request.SortBy))
//                {
//                    return sort.Descending(SortSpecialField.Score);
//                }

//                switch (request.SortBy.ToLower())
//                {
//                    case "name":
//                        if (request.SortAscending)
//                            return sort.Ascending(f => f.Name.Suffix("keyword"));
//                        else
//                            return sort.Descending(f => f.Name.Suffix("keyword"));

//                    case "price":
//                        if (request.SortAscending)
//                            return sort.Ascending(f => f.Price);
//                        else
//                            return sort.Descending(f => f.Price);

//                    case "created":
//                        if (request.SortAscending)
//                            return sort.Ascending(f => f.CreatedAt);
//                        else
//                            return sort.Descending(f => f.CreatedAt);

//                    case "rating":
//                        if (request.SortAscending)
//                            return sort.Ascending(f => f.AverageRating);
//                        else
//                            return sort.Descending(f => f.AverageRating);

//                    default:
//                        return sort.Descending(SortSpecialField.Score);
//                }
//            };
//        }

//        // روش 2: برگرداندن Action<SortDescriptor<ProductDocument>> (ساده‌تر)
//        public static Action<SortDescriptor<ProductDocument>> BuildSortAction(ProductSearchRequest request)
//        {
//            return sort => {
//                if (string.IsNullOrWhiteSpace(request.SortBy))
//                {
//                    sort.Descending(SortSpecialField.Score);
//                    return;
//                }

//                switch (request.SortBy.ToLower())
//                {
//                    case "name":
//                        if (request.SortAscending)
//                            sort.Ascending(f => f.Name.Suffix("keyword"));
//                        else
//                            sort.Descending(f => f.Name.Suffix("keyword"));
//                        break;

//                    case "price":
//                        if (request.SortAscending)
//                            sort.Ascending(f => f.Price);
//                        else
//                            sort.Descending(f => f.Price);
//                        break;

//                    case "created":
//                        if (request.SortAscending)
//                            sort.Ascending(f => f.CreatedAt);
//                        else
//                            sort.Descending(f => f.CreatedAt);
//                        break;

//                    case "rating":
//                        if (request.SortAscending)
//                            sort.Ascending(f => f.AverageRating);
//                        else
//                            sort.Descending(f => f.AverageRating);
//                        break;

//                    default:
//                        sort.Descending(SortSpecialField.Score);
//                        break;
//                }
//            };
//        }

//        // روش 3: برگرداندن IList<ISort> با استفاده از Sort<T>
//        public static IList<ISort> BuildSortList(ProductSearchRequest request)
//        {
//            var sorts = new List<ISort>();

//            if (string.IsNullOrWhiteSpace(request.SortBy))
//            {
//                sorts.Add(Sort<ProductDocument>.Descending(SortSpecialField.Score));
//                return sorts;
//            }

//            switch (request.SortBy.ToLower())
//            {
//                case "name":
//                    if (request.SortAscending)
//                        sorts.Add(Sort<ProductDocument>.Ascending(f => f.Name.Suffix("keyword")));
//                    else
//                        sorts.Add(Sort<ProductDocument>.Descending(f => f.Name.Suffix("keyword")));
//                    break;

//                case "price":
//                    if (request.SortAscending)
//                        sorts.Add(Sort<ProductDocument>.Ascending(f => f.Price));
//                    else
//                        sorts.Add(Sort<ProductDocument>.Descending(f => f.Price));
//                    break;

//                case "created":
//                    if (request.SortAscending)
//                        sorts.Add(Sort<ProductDocument>.Ascending(f => f.CreatedAt));
//                    else
//                        sorts.Add(Sort<ProductDocument>.Descending(f => f.CreatedAt));
//                    break;

//                case "rating":
//                    if (request.SortAscending)
//                        sorts.Add(Sort<ProductDocument>.Ascending(f => f.AverageRating));
//                    else
//                        sorts.Add(Sort<ProductDocument>.Descending(f => f.AverageRating));
//                    break;

//                default:
//                    sorts.Add(Sort<ProductDocument>.Descending(SortSpecialField.Score));
//                    break;
//            }

//            return sorts;
//        }
//    }

   
//}