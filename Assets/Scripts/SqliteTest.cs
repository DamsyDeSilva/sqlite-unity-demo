using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;

public class SqliteTest : MonoBehaviour
{
	public Text UI_data;

	private const string DATABASE_NAME = "my_test_db.db";

	private string EDITOR_DB_PATH;
	private string STRAMING_DB_PATH_ANDROID;
	private string STRAMING_DB_PATH_IOS;
	private string PERSISTENT_DB_PATH;

	public string db_path;
    public string db_connection_string;
    public IDbConnection db_connection;

	private const string CREATURES_TABLE_NAME = "Creatures";

    private void Awake()
    {

		EDITOR_DB_PATH = $"{Application.dataPath}/StreamingAssets/{DATABASE_NAME}";
		STRAMING_DB_PATH_ANDROID = $"{Application.streamingAssetsPath}/{DATABASE_NAME}";
		STRAMING_DB_PATH_IOS = $"{Application.dataPath}/Raw/{DATABASE_NAME}";
		PERSISTENT_DB_PATH = $"{Application.persistentDataPath}/{DATABASE_NAME}";
	}

    void Start()
    {
		LoadDataBase();
		SetDBPath();
		InitDBConnection();
	}

	private void  LoadDataBase()
    {
		// check if file exists in Application.persistentDataPath
		// if it doesn't -> open StreamingAssets directory and load the db

		if (File.Exists(PERSISTENT_DB_PATH))
		{
			return;
		}
		
		// saving the db from stramingAssests to persistent path
		if (Application.platform == RuntimePlatform.Android)
		{
            WWW reader = new WWW(STRAMING_DB_PATH_ANDROID);
			while (!reader.isDone) {}
			File.WriteAllBytes(PERSISTENT_DB_PATH, reader.bytes);
			return;
		}

		else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
			File.Copy(STRAMING_DB_PATH_IOS, STRAMING_DB_PATH_ANDROID);
		}

	}

	private void SetDBPath()
    {
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			db_path = EDITOR_DB_PATH;
		}
		else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
			db_path = PERSISTENT_DB_PATH;
		}
	}


	private void InitDBConnection()
    {
		db_connection_string = $"URI=file:{db_path}";
		db_connection = new SqliteConnection(db_connection_string);
		db_connection.Open();

	}

	private List<Creature> getAllCreatureDatafromDB()
    {
		string query = $"SELECT * FROM {CREATURES_TABLE_NAME}";

		IDbCommand dbCommand = db_connection.CreateCommand();
		dbCommand.CommandText = query;

		IDataReader reader = dbCommand.ExecuteReader();
		List<Creature> creaturesList = new List<Creature>();

		while (reader.Read())
		{
			Creature creature = new Creature(	reader.GetString(0), 
												reader.GetString(1), 
												reader.GetString(2), 
												reader.GetString(3)
												);

			creaturesList.Add(creature);
		}
		return creaturesList;
	}

	public void showAllCreatureData()
    {
		
		List<Creature> creaturesList = getAllCreatureDatafromDB();
		UI_data.text = creaturesList.Count.ToString();
		string ui_text = "";
		foreach (Creature creature in creaturesList)
        {
            ui_text += $"{creature.id} - {creature.name} - {creature.description} - {creature.condition}\n\n";

		}
		UI_data.text = ui_text;

	}

    private void OnDestroy()
    {
		db_connection.Close();
	}

	
}

public class Creature
{
	public string id;
	public string name;
	public string description;
	public string condition;

	public Creature(string id, string name, string description, string condition)
	{
		this.id = id;
		this.name = name;
		this.description = description;
		this.condition = condition;
	}
}


