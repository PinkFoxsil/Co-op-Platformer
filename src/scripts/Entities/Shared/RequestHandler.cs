using System;
using System.Collections.Generic;
using System.Linq;

public class RequestHandler<T>
{
	private readonly List<T> _requests = new();

	private readonly Func<T, int> _prioritySelector;
	private readonly Func<IEnumerable<T>, T> _accumulator;

	public RequestHandler(Func<T, int> prioritySelector, Func<IEnumerable<T>, T> accumulator)
	{
		_prioritySelector = prioritySelector;
		_accumulator = accumulator;
	}

	public T Request(T request)
	{
		_requests.Add(request);
		return request;
	}

	public void Remove(T request)
	{
		_requests.Remove(request);
	}

	public void Clear()
	{
		_requests.Clear();
	}

	public void ClearSource(Func<T, bool> predicate)
	{
		_requests.RemoveAll(r => predicate(r));
	}

	public void Tick(float dt, Func<T, Timer> timerSelector)
	{
		foreach (var request in _requests)
		{
			timerSelector(request)?.Tick(dt);
		}
	}

	public void Cleanup(Func<T, bool> persistentSelector, Func<T, Timer> timerSelector)
	{
		_requests.RemoveAll(r => timerSelector(r)?.HasStopped == true);

		_requests.RemoveAll(r => !persistentSelector(r));
	}

	public T Resolve(T fallback)
	{
		if (_requests.Count == 0)
		{
			return fallback;
		}
		
		int highest = _requests.Max(_prioritySelector);
		var selected = _requests.Where(r => _prioritySelector(r) == highest);

		return _accumulator(selected);
	}
}
