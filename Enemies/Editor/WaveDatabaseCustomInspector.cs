using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaveDatabase))]
public class WaveDatabaseCustomInspector : Editor
{
    WaveDatabase waveDatabase;

    void OnEnable()
    {
        waveDatabase = (target as WaveDatabase);
    }

    public override void OnInspectorGUI()
    {
        this.DrawDefaultInspector();
        DrawWaveDesigner();
    }

    Vector2 scrollPosition;
    public void DrawWaveDesigner()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(500));
        for (int i = 0; i < waveDatabase.waves.Count; i++)
        {
            WaveDatabase.Wave selWave = waveDatabase.waves[i];

            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Wave " + i);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X"))
            {
                if (EditorUtility.DisplayDialog("Delete Wave", "Permanently Deleting Wave.", "Ok", "Cancel"))
                {
                    waveDatabase.waves.RemoveAt(i);
                }
            }
            GUILayout.EndHorizontal();

            for (int j = 0; j < selWave.waveEnemies.Count; j++)
            {
                WaveDatabase.WaveEnemy selWaveEnemy = selWave.waveEnemies[j];

                GUILayout.BeginHorizontal("box");
                GUILayout.Label("Enemy Index");
                selWaveEnemy.index = EditorGUILayout.IntField(selWaveEnemy.index);
                GUILayout.Label("Delay");
                selWaveEnemy.delay = EditorGUILayout.FloatField(selWaveEnemy.delay);
                if (GUILayout.Button("X"))
                {       if (EditorUtility.DisplayDialog("Delete Enemy", "Permanently Deleting Enemy.", "Ok", "Cancel"))
                    {
                        selWave.waveEnemies.RemoveAt(j);
                    }
                }
                GUILayout.EndHorizontal();
            }
            if(GUILayout.Button("+", GUILayout.Width(30))) { selWave.waveEnemies.Add(new WaveDatabase.WaveEnemy()); }

            GUILayout.EndVertical();
            GUILayout.Space(1);
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add Wave")) { waveDatabase.waves.Add(new WaveDatabase.Wave()); };
    }
}
