using System;
using System.Collections.Generic;
namespace com.fpnn.rtm
{
	public class CacheNode
	{
		long mid;
		long from;

		public CacheNode(long mid, long from)
		{
			this.mid = mid;
			this.from = from;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if ((obj.GetType().Equals(this.GetType())) == false)
			{
				return false;
			}
			CacheNode temp = null;
			temp = (CacheNode)obj;

			return this.mid.Equals(temp.mid) && this.from.Equals(temp.from);

		}

		public override int GetHashCode()
		{
			return this.mid.GetHashCode() + this.from.GetHashCode();
		}
	}

	public class RTMPushCache
	{
		private int max;
		private Dictionary<CacheNode, bool> cache;
		private List<KeyValuePair<CacheNode, bool>> orderList;

		public RTMPushCache(int max)
		{
			this.max = max;
			this.cache = new Dictionary<CacheNode, bool>(max);
			this.orderList = new List<KeyValuePair<CacheNode, bool>>(max);
		}

		public bool containsKey(long mid, long from)
		{
			CacheNode key = new CacheNode(mid, from);
			bool ret = false;

			if (this.cache.ContainsKey(key))
				ret = true;
			else
				this.add(key);

			return ret;
		}

		private void add(CacheNode key)
		{
			if (this.cache.Count == max)
			{
				var toRemove = this.orderList[0];
				this.cache.Remove(toRemove.Key);
				this.orderList.Remove(toRemove);
			}
			this.orderList.Add(new KeyValuePair<CacheNode, bool>(key, true));
			this.cache[key] = true;
		}
	}
}

