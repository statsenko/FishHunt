using UnityEngine;
using System.Collections;

public class Json2SFSParser 
{
	/*
	static	public ISFSObject Parse(string json_text)
	{
		ISFSObject resultObj = new SFSObject();
		JSONObject j = new JSONObject(json_text);
		resultObj = parseObject(j, resultObj);

		return resultObj;
	}

	static	ISFSObject parseObject(JSONObject obj, ISFSObject sfsObj)
	{
		switch(obj.type)
		{
		case JSONObject.Type.OBJECT:
			for(int i = 0; i < obj.list.Count; i++)
			{
				ArrayList keys = obj.keys;
				if (keys.Count != 0)
				{
					string key = (string)obj.keys[i];
					JSONObject j = (JSONObject)obj.list[i];
					switch (j.type)
					{
					case JSONObject.Type.STRING:
						sfsObj.PutUtfString(key, j.str);
						break;
					case JSONObject.Type.NUMBER:
						sfsObj.PutInt(key, (int)j.n);
						break;
					case JSONObject.Type.BOOL:
						sfsObj.PutBool(key, j.b);
						break;
					case JSONObject.Type.ARRAY:
					{
						ISFSArray sfsArr = new SFSArray();
						JSONObject.Type type = JSONObject.Type.NULL;
						sfsArr = parseArray(j, sfsArr, out type);
						sfsObj.PutSFSArray(key, sfsArr);
						break;
					}
					case JSONObject.Type.OBJECT:
					{
						ISFSObject newObj = new SFSObject();
						parseObject(j, newObj);
						sfsObj.PutSFSObject(key, newObj); 
						break;
					}
					}
				}
			}
			break;
		}
		
		return sfsObj;
	}

	static	ISFSArray parseArray(JSONObject obj, ISFSArray sfsArr, out JSONObject.Type result)
	{
		result = JSONObject.Type.NULL;
		
		for(int i = 0; i < obj.list.Count; i++)
		{
			JSONObject j = (JSONObject)obj.list[i];
			switch (j.type)
			{
			case JSONObject.Type.STRING:
				sfsArr.AddUtfString( j.str);
				result = JSONObject.Type.STRING;
				break;
			case JSONObject.Type.NUMBER:
				sfsArr.AddInt( (int)j.n);
				result = JSONObject.Type.NUMBER;
				break;
			case JSONObject.Type.ARRAY:
			{
				ISFSArray newArr = new SFSArray();
				newArr = parseArray(j, newArr, out result);
				
				if (result == JSONObject.Type.NUMBER)
				{
					int[] arrInt = new int[newArr.Size()];
					for (int index = 0; index < newArr.Size(); ++index)
						arrInt[index] = newArr.GetInt(index);
					
					sfsArr.AddIntArray(arrInt);
				}
				break;
			}
			case JSONObject.Type.OBJECT:
			{
				SFSObject newObj = new SFSObject();
				parseObject(j, newObj);
				sfsArr.AddSFSObject(newObj); 
				result = JSONObject.Type.OBJECT;
				break;
			}
			}
		}
		
		return sfsArr;
	}
	*/
}
