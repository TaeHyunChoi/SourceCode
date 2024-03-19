using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using CDataStructure;

/// <summary>
/// [��ǥ] CSV ������ �ҷ����� ����ȭ (�ӵ� ����� ���Ͽ� �������� ��������� ����)
/// [���] (1)���� string �� byte[] �� �о���� (2)Generic<T> Ȱ���Ͽ� �ڵ� ���뼺 ���̱�
/// [���] Job System�� ����Ͽ� ��Ƽ ���������� CSV�� �ҷ����� ������ 'reference type�� Job struct���� ����� �� ����'�� ������ �־� �Ⱒ.
/// </summary>
public class DataTable
{
    private static List<SkillData> skillTable;
    private static List<ItemData>  itemTable;
    private static List<UnitData>  unitTalbe;
    private static List<MapData>   mapTable;

    public static List<SkillData> SkillTable { get => skillTable; }
    public static List<ItemData>  ItemTable  { get => itemTable; }
    public static List<UnitData>  UnitTable  { get => unitTalbe; }
    public static List<MapData>   MapTable     { get => mapTable; }


#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_WIN
    //.csv (��ȹ��, �����Ϳ��� .bin ���Ϸ� ��ȯ �ʿ�)
    public static void LoadCSVTable()
    {
        skillTable = LoadTable<SkillData>("SkillData");
        itemTable = LoadTable<ItemData>("ItemData");
        unitTalbe = LoadTable<UnitData>("UnitData");
        mapTable = LoadTable<MapData>("MapData");
    }
    private static List<T> LoadTable<T>(string fileName) where T : IDataSetter, new()
    {
        List<Dictionary<string, string>> table = new List<Dictionary<string, string>>();
        TextAsset csv = Resources.Load<TextAsset>("CSV/" + fileName);
        StringReader reader = new StringReader(csv.text);
        StringBuilder sb = new StringBuilder();

        //Setting
        string[] columns;   //Į����
        int index;          //Į����[] �ε���
        string line;        //�� ��
        char[] chars;       //�� ���� char ���·� �ɰ� (�߰� ,�� �߶󳻱� ����)
        bool isSplit;       //�з� ���� (��� �� ������ ,�� CSV ���н�ǥ�� �����ϱ� ����)

        //Column Index
        line = reader.ReadLine(); //ù�� ������
        columns = line.Split(',');

        //Content
        while (true)
        {
            line = reader.ReadLine();
            if (line == null)
                break;

            Dictionary<string, string> data = new Dictionary<string, string>();
            chars = line.ToCharArray();
            isSplit = true;
            index = -1;

            for (int i = 0; i < chars.Length; ++i)
            {
                //������ �߰��� ,�� ������ �ʱ� ���� �Ǻ� ���� �߰�
                if (chars[i] == '\u0022') //ū����ǥ(")�� �����ڵ�
                {
                    isSplit = !isSplit;
                    continue;
                }

                if (isSplit
                    && chars[i] == '\u002C') //��ǥ(,) �����ڵ�
                {
                    data.Add(columns[++index], sb.ToString());
                    sb.Clear();
                    continue;
                }

                sb.Append(chars[i]);
            }

            //������ ������ �߰� (,�� ��� ������ �Ȱɸ�)
            data.Add(columns[++index], sb.ToString());
            table.Add(data);
            sb.Clear();
        }

        List<T> list = new List<T>();
        for (int i = 0; i < table.Count; ++i)
        {
            T tData = new T();
            tData.Set(table[i]);
            list.Add(tData);
        }

        return list;
    }


    // map sampling
    public static void WriteBinaryMappingData<T>(Dictionary<int, T> data, string fileName) where T:struct
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        string filePath = Path.Combine(Application.dataPath, "Resources", "bin", "MapVoxelData", fileName + ".dat");
        FileStream fileStream = File.Create(filePath);

        // Dictionary ����ȭ
        binaryFormatter.Serialize(fileStream, data);

        fileStream.Close();
    }
    public static Dictionary<int, T> LoadMappingData<T>(string fileName) where T:struct
    {
        string filePath = Path.Combine(Application.dataPath, "Resources", "bin", "MapVoxelData", fileName + ".dat");
        if (File.Exists(filePath))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Open(filePath, FileMode.Open);

            // ���Ͽ��� �����͸� ������ȭ�Ͽ� Dictionary�� �ε�
            Dictionary<int, T> map = (Dictionary<int, T>)binaryFormatter.Deserialize(fileStream);

            fileStream.Close();
            return map;
        }
        else
        {
            Debug.LogError("������ �������� �ʽ��ϴ�.");
        }

        return null;
    }
    //public static void WriteBinaryMapVoxel(Dictionary<int, Voxel_t> data, string fileName)
    //{
    //    BinaryFormatter binaryFormatter = new BinaryFormatter();
    //    string filePath = Path.Combine(Application.dataPath, "Resources", "bin", "MapVoxelData", fileName + ".dat");
    //    FileStream fileStream = File.Create(filePath);

    //    // Dictionary ����ȭ
    //    binaryFormatter.Serialize(fileStream, data);

    //    fileStream.Close();
    //}
    //public static Dictionary<int, Voxel_t> LoadMapVoxel(string fileName)
    //{
    //    string filePath = Path.Combine(Application.dataPath, "Resources", "bin", "MapVoxelData", fileName + ".dat");
    //    if (File.Exists(filePath))
    //    {
    //        BinaryFormatter binaryFormatter = new BinaryFormatter();
    //        FileStream fileStream = File.Open(filePath, FileMode.Open);

    //        // ���Ͽ��� �����͸� ������ȭ�Ͽ� Dictionary�� �ε�
    //        Dictionary<int, Voxel_t> data = (Dictionary<int, Voxel_t>)binaryFormatter.Deserialize(fileStream);

    //        fileStream.Close();
    //        return data;
    //    }
    //    else
    //    {
    //        Debug.LogError("������ �������� �ʽ��ϴ�.");
    //    }

    //    return null;
    //}

#endif

    //.bin (���α׷���, .bin ���Ϸ� ���������̺� �б�)
    public static void LoadTable()
    {
        skillTable = ReadBinary<SkillData>("SkillData.bin");
        itemTable = ReadBinary<ItemData>("ItemData.bin");
        unitTalbe = ReadBinary<UnitData>("UnitData.bin");
        mapTable = ReadBinary<MapData>("MapData.bin");
    }

    public static List<T> ReadBinary<T>(string fileName) where T : struct, IDataSetter
    {
        string path = Application.dataPath + "/Resources/bin/" + fileName;

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);
        List<T> table = (List<T>)formatter.Deserialize(stream);
        stream.Close();

        return table;
    }
    public static void WriteBinaryFiles()
    {
        string path = Application.dataPath + "/Resources/bin/";

        WriteBinary(path + "SkillData.bin", SkillTable);
        WriteBinary(path + "ItemData.bin", ItemTable);
        WriteBinary(path + "UnitData.bin", UnitTable);
        WriteBinary(path + "MapData.bin", MapTable);
    }
    private static void WriteBinary<T>(string path, List<T> table) where T : struct, IDataSetter
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, table);
        stream.Close();
    }
}