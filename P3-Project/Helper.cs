namespace P3_Project.Helpers;
using System.Collections;

public class Enumerate<T, U> : IEnumerable<(int, U)>, IEnumerator<(int, U)> where T: IEnumerable<U>{
	int count = -1;
	IEnumerator<U> item;
	
	public (int, U) Current => (count, item.Current);
	object IEnumerator.Current => Current;
	public IEnumerator<(int, U)> GetEnumerator() => this;
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	void IDisposable.Dispose() {}

	public Enumerate(T obj){
		item = obj.GetEnumerator();
	}

	public void Reset() {
		item.Reset();
		count = -1;
	}

	public bool MoveNext() {
		count++;
		return item.MoveNext();
	}
}

public class Counter : IEnumerable<int>, IEnumerator<int> {
	
	int f;
	int t;

	public Counter(int fromval, int toval){
		f = fromval - 1;
		t = toval;
		Current = f;
	}

	public Counter(int toval){
		f = -1;
		t = toval;
		Current = f;
	}

	public int Current {get; set;}
	object IEnumerator.Current => Current;
	public IEnumerator<int>GetEnumerator() => this;
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	void IDisposable.Dispose() {}

	public void Reset() {
		Current = f;
	}

	public bool MoveNext() {
		Current++;
		return Current < t;
	}

}

