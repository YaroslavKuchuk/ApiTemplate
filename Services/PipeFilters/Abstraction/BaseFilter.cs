using System.Collections;

namespace Services.PipeFilters.Abstraction
{
    public abstract class BaseFilter<T> : IFilter<T> where T : class, IList
	{
		private IFilter<T> _next;

		protected abstract T Process(T input);

		public T Execute(T input)
		{
			T val = Process(input);

			if (_next != null)
			{
				val = _next.Execute(val);
			}

			return val;
		}

		public void Register(IFilter<T> filter)
		{
			if (_next == null)
			{
				_next = filter;
			}
			else
			{
				_next.Register(filter);
			}
		}
	}


}
