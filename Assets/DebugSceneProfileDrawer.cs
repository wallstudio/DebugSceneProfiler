using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class DebugSceneProfileDrawer : MonoBehaviour
{
    IEnumerator Start()
    {
        while(true)
        {
            yield return StartCoroutine(_Start());
        }
    }
    IEnumerator _Start()
    {
        DebugSceneProfiler DP;
        using(var dp = new DebugSceneProfiler("A Profile"))
        {
            yield return new WaitForSeconds(3.3f);
            using(var _dp = new DebugSceneProfiler("B Profile"))
            {
                yield return new WaitForSeconds(0.3f);
                _dp.Lap("B-0");
                yield return new WaitForSeconds(0.3f);
                _dp.Lap("B-1");
                yield return new WaitForSeconds(0.3f);
                _dp.Lap("B-2");
                DP = new DebugSceneProfiler("D Profile");
                yield return new WaitForSeconds(0.3f);
            }
            dp.Lap("A-0");
            yield return new WaitForSeconds(0.3f);
            DP.Lap("D-0");
        }
        yield return new WaitForSeconds(0.3f);
        DP.Lap("D-1");
        using(var dp = new DebugSceneProfiler("E Profile"))
        {
            yield return new WaitForSeconds(0.3f);
            DP.Lap("D-2");
            yield return new WaitForSeconds(0.3F);
            DP.Dispose();
            yield return new WaitForSeconds(0.3F);
        }
        using(var dp = new DebugSceneProfiler("C Profile"))
        {
            yield return new WaitForSeconds(0.2f);
            dp.Lap("C-0");
            yield return new WaitForSeconds(0.2f);
            dp.Lap("C-1");
            yield return new WaitForSeconds(0.2f);
            dp.Lap("C-2");
            yield return new WaitForSeconds(0.2f);
            dp.Lap("C-3");
            yield return new WaitForSeconds(0.2f);
        }
    }


    [SerializeField] List<DebugSceneProfiler> profiles = DebugSceneProfiler.profiles;
    [SerializeField] RectInt windowRect = new RectInt(10, 10, 400, 400); 
    GUIStyle borderStyle;
    GUIStyle profStyle;
    List<GUIStyle> lapStyles;

    void OnGUI()
    {
        // テクスチャの準備
        if(borderStyle == null || profStyle == null || lapStyles == null || lapStyles.Count == 0)
        {
            borderStyle = CreateColorBoxStyle(Color.black);
            profStyle = CreateColorBoxStyle(Color.HSVToRGB(0f, 0f, 0.75f), 0.5f);
            const int STYLE_COUNT = 5;
            const float S = 0.5f;
            const float V = 1f;
            lapStyles = Enumerable.Range(0, STYLE_COUNT)
                .Select(i => CreateColorBoxStyle(Color.HSVToRGB(i / (float)STYLE_COUNT, S, V), 0.5f))
                .ToList();
        }

        const int WINDOW_LABEL_H = 30;
        const int WINDOW_HEADER_H = 50;
        const int PROFILE_HEADER_H = 20;
        const int LAP_H = 15;
        Rect window = GUILayout.Window(0, new Rect(windowRect.position, windowRect.size), id =>
        {
            if(profiles == null || profiles.Count == 0)
            {
                GUILayout.Label("プロファイルはありません");
                return;
            }

            RectInt contentsRect = new RectInt(Vector2Int.one * 10, windowRect.size - new Vector2Int(100, 30));
            double totalSpan = profiles.Select(p => p.endTime).Max() - profiles.Select(p => p.startTime).Min();
            Vector2Int contentsOffset = contentsRect.position;
            profiles.Sort((a, b) => a.startTime.CompareTo(b.startTime));
            Vector2Int lastRightTop = Vector2Int.zero;
            int bottom = 0;

            // 罫線
            const int MIN_COLUMN_WIDTH = 10;
            double columnSeconds = 1 / 100f / 10f; // 最小10ms刻み
            double columnWidth = 0;
            while(columnWidth < MIN_COLUMN_WIDTH && contentsRect.width > MIN_COLUMN_WIDTH && columnSeconds < 3600)
            {
                columnSeconds *= 10;
                columnWidth = contentsRect.width / totalSpan * columnSeconds;
            }
            for(int i = 0, il = Mathf.CeilToInt((float)(totalSpan / columnSeconds)); i < il; i++)
            {
                Rect borderRect = new Rect(
                    x: (int)(contentsRect.x + i * columnWidth),
                    y: contentsRect.y,
                    width: i % 10 == 0 ? 4 : 1,
                    height: contentsRect.height);
                GUI.Box(borderRect, "", borderStyle);
            }
         
             GUILayout.BeginHorizontal();
            windowRect = new RectInt(
                xMin: windowRect.x,
                yMin: windowRect.y,
                width: (int)GUILayout.HorizontalSlider(windowRect.width, Screen.width * 0.8f, Screen.width * 10, GUILayout.Width(100)),
                height: windowRect.height);
            GUILayout.Label("scale(拡大するとドローコールが跳ね上がるので注意)");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            contentsOffset += new Vector2Int(0, WINDOW_HEADER_H);
            Vector2Int graphOffset = contentsOffset;

            for(int i = 0, il = profiles.Count; i < il; i++)
            {
                // 各プロファイルの描画
                graphOffset *= new Vector2Int(0, 1);
                double startTimeRate = profiles[i].startTime / totalSpan;
                double spanTimeRate = profiles[i].spanTime / totalSpan;
                graphOffset = new Vector2Int((int)(contentsRect.x + startTimeRate * contentsRect.width), graphOffset.y);
                // 重なりが無ければ上に詰める
                if(graphOffset.x >= lastRightTop.x && contentsOffset.y < lastRightTop.y)
                {
                    graphOffset = new Vector2Int(graphOffset.x, contentsOffset.y);
                }
                Rect profileRect = new Rect(
                    x: graphOffset.x,
                    y: graphOffset.y,
                    width: (int)(spanTimeRate * contentsRect.width),
                    height: profiles[i].laps.Count * LAP_H + PROFILE_HEADER_H);
                Rect meginedProfileRect = new Rect(profileRect.position + Vector2.one, profileRect.size - Vector2.one * 2);
                if(GUI.Button(meginedProfileRect, profiles[i].label, profStyle))
                {
                    Debug.Log(profiles[i]);
                }
                graphOffset += new Vector2Int(0, PROFILE_HEADER_H);
                if(lastRightTop.x < profileRect.xMax)
                {
                    lastRightTop = new Vector2Int((int)profileRect.xMax, (int)profileRect.yMin);
                }
                if(bottom < profileRect.yMax)
                {
                    bottom = (int)profileRect.yMax;
                }

                
                for(int j = 0, jl = profiles[i].laps.Count; j < jl; j++)
                {
                    // 各ラップタイムの描画
                    double lapStartTimeRate = profiles[i].laps[j].startTime / totalSpan;
                    double lapSpanTimeRate = profiles[i].laps[j].spanTime / totalSpan;
                    graphOffset = new Vector2Int((int)(contentsRect.x + lapStartTimeRate * contentsRect.width), graphOffset.y);
                    Rect lapRect = new Rect(
                        x: graphOffset.x,
                        y: graphOffset.y,
                        width: (int)(lapSpanTimeRate * contentsRect.width),
                        height: LAP_H);
                    if(GUI.Button(lapRect, profiles[i].laps[j].label, lapStyles[j % lapStyles.Count]))
                    {
                        Debug.Log(profiles[i].laps[j]);
                    }
                    graphOffset += new Vector2Int(0, LAP_H);
                }
            }

            windowRect.height = WINDOW_LABEL_H + WINDOW_HEADER_H + bottom;
        }, "シーンプロファイラ");
    }

    static GUIStyle CreateColorBoxStyle(Color color, float alpha = -1)
    {
        if(alpha >= 0)
        {
            color = new Color(color.r, color.g, color.b, alpha);
        }

        GUIStyle profStyle;
        var tex = new Texture2D(1, 1, GraphicsFormat.R8G8B8A8_SRGB, TextureCreationFlags.None);
        tex.SetPixel(0, 0, color);
        tex.Apply();
        profStyle = new GUIStyle()
        {
            normal = new GUIStyleState()
            {
                background = tex,
            }
        };
        return profStyle;
    }
}
