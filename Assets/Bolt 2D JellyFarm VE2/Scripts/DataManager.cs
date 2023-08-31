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
    //[���׷��̵� �ý���]
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
            GameManager.instance.num_level = 1; //[���׷��̵� �ý���]
            GameManager.instance.click_level = 1; //[���׷��̵� �ý���]
            JsonSave();

            // Debug.Log("path ����"); [���׷��̵� �ý���] ����
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
                GameManager.instance.num_level = save_data.num_level; //[���׷��̵� �ý���]
                GameManager.instance.click_level = save_data.click_level; //[���׷��̵�ý���]

              //  Debug.Log("���̺� �ε� �Ϸ�"); [���׷��̵� �ý���] ����
            }

        }
    }

    public void JsonSave()//���������� �������Ʈ ������ ����
    {
        SaveData save_data = new SaveData();

        for (int i = 0; i < GameManager.instance.jelly_list.Count; ++i)
        {
            Jelly jelly = GameManager.instance.jelly_list[i].GetComponent<Jelly>(); //Jelly jelly = GameManager.instance.jelly_list[i]; //[���׷��̵� �ý���] ����: Jelly jelly = GameManager.instance.jelly_list[i].GetComponent<Jelly>();
            save_data.jelly_list.Add(new Data(jelly.gameObject.transform.position, jelly.id, jelly.level, jelly.exp));
        }
        for (int i = 0; i < GameManager.instance.jelly_unlock_list.Length; ++i)
            save_data.jelly_unlock_list[i] = GameManager.instance.jelly_unlock_list[i];
        save_data.jelatin = GameManager.instance.jelatin;
        save_data.gold = GameManager.instance.gold;
        save_data.num_level = GameManager.instance.num_level; //[���׷��̵� �ý���]
        save_data.click_level = GameManager.instance.click_level; //[���׷��̵� �ý���]

        string json = JsonUtility.ToJson(save_data, true);

        File.WriteAllText(path, json);
    }
}
