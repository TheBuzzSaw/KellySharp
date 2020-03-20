using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KellySharp
{
    public class QueryBuilder
    {
        [Flags]
        public enum Formatting
        {
            None = 0,
            WrapJoins = 1 << 0 // For MS Access
        }

        private string? _from = null;
        private string? _limit = null;
        private string? _offset = null;
        private readonly List<string> _specify = new List<string>();
        private readonly List<string> _select = new List<string>();
        private readonly List<string> _join = new List<string>();
        private readonly List<string> _where = new List<string>();
        private readonly List<string> _groupBy = new List<string>();
        private readonly List<string> _having = new List<string>();
        private readonly List<string> _orderBy = new List<string>();

        public QueryBuilder()
        {
        }

        public QueryBuilder From(string? from)
        {
            _from = from;
            return this;
        }

        public QueryBuilder Limit(string? limit)
        {
            _limit = limit;
            return this;
        }

        public QueryBuilder Offset(string? offset)
        {
            _offset = offset;
            return this;
        }

        public QueryBuilder Join(string joinType, string joinTable, string onClause)
        {
            _join.Add($"{joinType} {joinTable} ON {onClause}");
            return this;
        }

        public QueryBuilder Specify(params string[] items) => Specify((IEnumerable<string>)items);
        public QueryBuilder Specify(IEnumerable<string> items)
        {
            _specify.AddRange(items);
            return this;
        }

        public QueryBuilder Select(params string[] items) => Select((IEnumerable<string>)items);
        public QueryBuilder Select(IEnumerable<string> items)
        {
            _select.AddRange(items);
            return this;
        }

        public QueryBuilder Where(params string[] items) => Where((IEnumerable<string>)items);
        public QueryBuilder Where(IEnumerable<string> items)
        {
            _select.AddRange(items);
            return this;
        }

        public QueryBuilder GroupBy(params string[] items) => GroupBy((IEnumerable<string>)items);
        public QueryBuilder GroupBy(IEnumerable<string> items)
        {
            _groupBy.AddRange(items);
            return this;
        }

        public QueryBuilder Having(params string[] items) => Having((IEnumerable<string>)items);
        public QueryBuilder Having(IEnumerable<string> items)
        {
            _having.AddRange(items);
            return this;
        }

        public QueryBuilder OrderBy(params string[] items) => OrderBy((IEnumerable<string>)items);
        public QueryBuilder OrderBy(IEnumerable<string> items)
        {
            _orderBy.AddRange(items);
            return this;
        }
        
        public override string ToString() => ToString(Formatting.None);
        
        public string ToString(Formatting formatting)
        {
            var builder = new StringBuilder("SELECT ");

            foreach (var item in _specify)
                builder.Append(item).Append(' ');

            if (_select.Count > 0)
                builder.AppendJoin(", ", _select);
            else
                builder.Append('*');

            builder.Append(" FROM ");

            bool wrapJoins = (formatting & Formatting.WrapJoins) == Formatting.WrapJoins && _join.Count > 0;
            
            if (wrapJoins)
            {
                for (int i = -1; i < _join.Count; ++i)
                    builder.Append('(');
            }
            
            builder.Append(_from ?? "NULL");

            if (wrapJoins)
                builder.Append(')');
            
            foreach (var item in _join)
            {
                builder.Append(' ').Append(item);

                if (wrapJoins)
                    builder.Append(')');
            }

            if (_where.Count > 0)
            {
                builder
                    .Append(" WHERE (")
                    .AppendJoin(") AND (", _where)
                    .Append(')');
            }

            if (_groupBy.Count > 0)
            {
                builder
                    .Append(" GROUP BY ")
                    .AppendJoin(", ", _groupBy);
            }

            if (_having.Count > 0)
            {
                builder
                    .Append(" HAVING (")
                    .AppendJoin(") AND (", _having)
                    .Append(')');
            }

            if (_orderBy.Count > 0)
            {
                builder
                    .Append(" ORDER BY ")
                    .AppendJoin(", ", _orderBy);
            }

            if (_limit != null)
            {
                builder.Append(" LIMIT ").Append(_limit);

                if (_offset != null)
                    builder.Append(" OFFSET ").Append(_offset);
            }
            
            return builder.ToString();
        }
    }
}