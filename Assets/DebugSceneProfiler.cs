using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DebugSceneProfiler : IDisposable
{
    static DateTime applicationStartTime;
    [RuntimeInitializeOnLoadMethod]
    static void SetApplicationStartTime(){ applicationStartTime = DateTime.Now; }

	public static List<DebugSceneProfiler> profiles = new List<DebugSceneProfiler>();

    public string label;
    public double startTime;
    public double endTime;
    public double spanTime;


    [Serializable]
    public class LapInfo
    {
        public string label;
        public double startTime;
		public double endTime;
        public double spanTime;

        public LapInfo(string label, double startTime, double endTime)
        {
            this.label = label;
            this.startTime = startTime;
            this.endTime = endTime;
            this.spanTime = endTime - startTime;
        }

		public override string ToString()
		{
			return JsonUtility.ToJson(this);
		}
    }
    public List<LapInfo> laps = new List<LapInfo>();


    public DebugSceneProfiler(string label = null)
    {
        this.label = label ?? string.Empty;
        startTime = (DateTime.Now - applicationStartTime).TotalSeconds;
		profiles.Add(this);
    }

    public void Dispose()
    {
        Lap("Rest...");
        endTime = (DateTime.Now - applicationStartTime).TotalSeconds;
        spanTime = endTime - startTime;
        label = string.Format("[{0:000.00}s] {1}", spanTime, label ?? string.Empty);
    }

    public void Lap(string label = null)
    {
        double lapStartTime = laps.Count == 0 ? startTime : laps.Last().endTime;
		double lapEndTime = (DateTime.Now - applicationStartTime).TotalSeconds;
        double lapSpanTime = lapEndTime - lapStartTime;
        LapInfo lap = new LapInfo(
			label: string.Format("[{0:000.00}s] {1}", lapSpanTime, label ?? string.Empty),
			startTime: lapStartTime,
			endTime: lapEndTime);
        laps.Add(lap);
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }
}