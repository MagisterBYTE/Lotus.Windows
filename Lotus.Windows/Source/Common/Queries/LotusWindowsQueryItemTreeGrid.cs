using System;

using Lotus.Core;

namespace Lotus.Windows
{
    /** \addtogroup WindowsCommonQueries
	*@{*/
    /// <summary>
    /// Строковый фильтр заголовка для <see cref="LotusTreeGrid"/>: значение читается из <see cref="ITreeGridNode.GetValue"/> по <see cref="QueryItem.PropertyName"/>.
    /// </summary>
    public class QueryItemTreeGridString : QueryItemString
    {
        /// <inheritdoc />
        public override bool MatchesFilter(object? item)
        {
            if (item is LotusTreeGridRowModel row)
            {
                var valueRaw = row.Node.GetValue(PropertyName);
                var value = valueRaw?.ToString();
                if (value is null) return false;

                return MatchString(value);
            }

            return base.MatchesFilter(item);
        }

        private bool MatchString(string value)
        {
            switch (_filterFunction)
            {
                case TFilterFunction.Equals:
                    return _searchValue == value;
                case TFilterFunction.NotEqual:
                    return _searchValue != value;
                case TFilterFunction.Contains:
                    return value.Contains(_searchValue, StringComparison.OrdinalIgnoreCase);
                case TFilterFunction.StartsWith:
                    return value.StartsWith(_searchValue, StringComparison.OrdinalIgnoreCase);
                case TFilterFunction.EndsWith:
                    return value.EndsWith(_searchValue, StringComparison.OrdinalIgnoreCase);
                case TFilterFunction.Like:
                    return value.Contains(_searchValue, StringComparison.OrdinalIgnoreCase);
                default:
                    return false;
            }
        }
    }

    /// <summary>
    /// Числовой фильтр заголовка для <see cref="LotusTreeGrid"/> по значению узла.
    /// </summary>
    public class QueryItemTreeGridNumber : QueryItemNumber
    {
        /// <inheritdoc />
        public override bool MatchesFilter(object? item)
        {
            if (item is LotusTreeGridRowModel row)
            {
                var valueRaw = row.Node.GetValue(PropertyName);
                if (valueRaw is null) return false;
                var value = XNumberConverter.ToDouble(valueRaw, double.MinValue);
                if (value == double.MinValue) return false;

                return MatchNumber(value);
            }

            return base.MatchesFilter(item);
        }

        private bool MatchNumber(double value)
        {
            switch (_filterFunction)
            {
                case TFilterFunction.Equals:
                    return Math.Abs(_comparisonValueLeft - value) < 0.000001;
                case TFilterFunction.NotEqual:
                    return Math.Abs(_comparisonValueLeft - value) >= 0.000001;
                case TFilterFunction.LessThan:
                    return _comparisonValueLeft < value;
                case TFilterFunction.LessThanOrEqual:
                    return _comparisonValueLeft <= value;
                case TFilterFunction.GreaterThan:
                    return _comparisonValueLeft > value;
                case TFilterFunction.GreaterThanOrEqual:
                    return _comparisonValueLeft >= value;
                case TFilterFunction.Between:
                    return _comparisonValueLeft <= value && _comparisonValueRight >= value;
                default:
                    return false;
            }
        }
    }
    /**@}*/
}
