using KellySharp;
using Xunit;

namespace KellyTest
{
    public class QueryBuilderTest
    {
        public const string Users = "users";
        public const string Articles = "articles";

        [Fact]
        public void BuildAccessQuery()
        {
            var query = new QueryBuilder()
                .From(Users + " u")
                .Join("INNER JOIN", Articles + " a", "a.user_id = u.user_id")
                .ToString(QueryBuilder.Formatting.WrapJoins);
            
            Assert.Equal(query, $"SELECT * FROM (({Users} u) INNER JOIN {Articles} a ON a.user_id = u.user_id)");
        }
    }
}