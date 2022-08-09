using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class GameData
{
    public int BGM_Volume = 0;
    public int Effect_Volume = 0;

    public int Gold = 0;
    public int Hp = 10;
    public float MoveSpeed = 5f;

    public List<MonsterData> monsterKillDatas;

    public GameData(int _gold, int _hp, float _moveSpeed)
    {
        Gold = _gold;
        Hp = _hp;
        MoveSpeed = _moveSpeed;
        monsterKillDatas = new List<MonsterData>();
    }
}


[Serializable]
public class MonsterData
{
    public int Index;
    public string Name;
    public float MoveSpeed;
    public float RoataionSpeed;
    public string Description;

    public MonsterData(int _index, string _name, float _moveSpeed, float _rotationSpeed, string _description)
    {
        Index = _index;
        Name = _name;
        MoveSpeed = _moveSpeed;
        RoataionSpeed = _rotationSpeed;
        Description = _description;
    }
}

public class DataManager : MonoBehaviour
{
    
    static GameObject container;
    static GameObject Container { get => container; }

    static DataManager instance;
    public static DataManager Instance
    {
        get
        {
            if(instance == null)
            {
                container = new GameObject();
                container.name = "DataManager";
                instance = container.AddComponent(typeof(DataManager)) as DataManager;

                instance.SetMonsterDadaFromCSV();

                DontDestroyOnLoad(container);
            }

            return instance;
        }
        
    }

    GameData gameDatas;
    public GameData GameData
    {
        get
        {
            if(gameDatas == null)
            {
                LoadGameData();
                SaveGameData();
            }

            return gameDatas;
        }
    }

    void InitGameData()
    {
        gameDatas = new GameData(100, 300, 5f);

        gameDatas.monsterKillDatas.Add(new MonsterData(1, "박도일", 2f, 1f, "일찍옴"));
        gameDatas.monsterKillDatas.Add(new MonsterData(2, "미래의 박도일", 2f, 1f, "내일 안옴"));
    }

    public void SaveGameData()
    {
        InitGameData();
        string _toJsonData = JsonUtility.ToJson(gameDatas, true);
        string _filePath = Application.persistentDataPath + GameDataFileName;
        File.WriteAllText(_filePath, _toJsonData);
    }

    public void LoadGameData()
    {
        string _filePath = Application.persistentDataPath + GameDataFileName;

        if (File.Exists(_filePath))
        {
            string formJsonData = File.ReadAllText(_filePath);
            gameDatas = JsonUtility.FromJson<GameData>(formJsonData);

            if(gameDatas == null)
            {
                InitGameData();
            }
        }
        else
        {
            InitGameData();
        }
    }

    public string GameDataFileName = ".json";

    [Header("몬스터 관련 DB")]
    [SerializeField] TextAsset monsterDB;
    public Dictionary<int ,MonsterData> MonsterDataDict { get; set; }

    private void SetMonsterDadaFromCSV()
    {
        monsterDB = Resources.Load<TextAsset>("CSV/GameData - Monster");

        if(monsterDB == null)
        {
            Debug.LogError("CSV/GameData - Monster 파일이 없음!!");
            return;
        }

        if(MonsterDataDict == null)
        {
            MonsterDataDict = new Dictionary<int, MonsterData>();
        }

        string[] lines = monsterDB.text.Substring(0, monsterDB.text.Length).Split('\n');
        for (int i = 1; i < lines.Length; ++i)
        {
            string[] row = lines[i].Split(',');
            MonsterDataDict.Add(int.Parse(row[0]), new MonsterData(
                int.Parse(row[0]), row[1], float.Parse(row[2]), float.Parse(row[3]), row[4]
                ));
        }
    }

    public MonsterData GetMonsterData(int _index)
    {
        if (MonsterDataDict.ContainsKey(_index))
        {
            return MonsterDataDict[_index];
        }

        Debug.LogWarning(_index + ", 해당 인덱스 데이터가 없음");
        return null;

    }
}
