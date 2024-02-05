﻿namespace Services.PipeFilters.Abstraction
{
	public interface IFilterChain<T>
	{
		T Execute(T input);
		IFilterChain<T> Register(IFilter<T> filter);
	}
}