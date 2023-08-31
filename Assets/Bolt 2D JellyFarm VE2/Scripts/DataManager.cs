using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
    public int jelatin;
    public int gold;
    public bool[] jelly_unlock_list = new bool[12];
    public List<Data> jelly_list = new List<Data>();
    //[업그레이드 시스템]
    public int num_level;
    public int click_level;
}
public class DataManager : MonoBehaviour
{
    string path;

    void Start()
    {
        path = Path.Combine(Application.dataPath, "database.json");
        JsonLoad();
    }

    public void JsonLoad()
    {
        SaveData save_data = new SaveData();

        if (!File.Exists(path))
        {
            GameManager.instance.jelatin = 0;
            GameManager.instance.gold = 0;
            GameManager.instance.num_level = 1; //[업그레이드 시스템]
            GameManager.instance.click_level = 1; //[업그레이드 시스템]
            JsonSave();

            // Debug.Log("path 없음"); [업그레이드 시스템] 삭제
        }
        else
        {
            string load_json = File.ReadAllText(path);
            save_data = JsonUtility.FromJson<SaveData>(load_json);

            if (save_data != null)
            {
                for (int i = 0; i < save_data.jelly_list.Count; ++i)
                    GameManager.instance.jelly_data_list.Add(save_data.jelly_list[i]);
                for (int i = 0; i < save_data.jelly_unlock_list.Length; ++i)
                    GameManager.instance.jelly_unlock_list[i] = save_data.jelly_unlock_list[i];
                GameManager.instance.jelatin = save_data.jelatin;
                GameManager.instance.gold = save_data.gold;
                GameManager.instance.num_level = save_data.num_level; //[업그레이드 시스템]
                GameManager.instance.click_level = save_data.click_level; //[업그레이드시스템]

              //  Debug.Log("세이브 로딩 완료"); [업그레이드 시스템] 삭제
            }

        }
    }

    public void JsonSave()//젤리정보와 언락리스트 정보를 저장
    {
        SaveData save_data = new SaveData();

        for (int i = 0; i < GameManager.instance.jelly_list.Count; ++i)
        {
            Jelly jelly = GameManager.instance.jelly_list[i].GetComponent<Jelly>(); //Jelly jelly = GameManager.instance.jelly_list[i]; //[업그레이드 시스템] 원래: Jelly jelly = GameManager.instance.jelly_list[i].GetComponent<Jelly>();
            save_data.jelly_list.Add(new Data(jelly.gameObject.transform.position, jelly.id, jelly.level, jelly.exp));
        }
        for (int i = 0; i < GameManager.instance.jelly_unlock_list.Length; ++i)
            save_data.jelly_unlock_list[i] = GameManager.instance.jelly_unlock_list[i];
        save_data.jelatin = GameManager.instance.jelatin;
        save_data.gold = GameManager.instance.gold;
        save_data.num_level = GameManager.instance.num_level; //[업그레이드 시스템]
        save_data.click_level = GameManager.instance.click_level; //[업그레이드 시스템]

        string json = JsonUtility.ToJson(save_data, true);

        File.WriteAllText(path, json);
    }
}
