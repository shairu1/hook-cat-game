using System.IO;
using System.Collections.Generic;

/*
    name может использовать: буквы, цифры, _
*/

namespace SHAI
{
    /// <summary> Сохранение данных. </summary>
    namespace SavingData
    {
        public class SavingData
        {
            public static readonly string extension = ".shai";

            private List<SObject> SObjects;

            public SavingData()
            {
                if (SObjects != null)
                {
                    for (int i = 0; i < SObjects.Count; i++)
                    {
                        SObjects[i].DeleteAllData();
                    }
                }
                SObjects = new List<SObject>();
            }

            public void Save(string objectName, string path, bool hyphenation = false)
            {
                string tabulation = "", newLine = "", save = "";

                if (hyphenation)
                {
                    tabulation = "\t";
                    newLine = System.Environment.NewLine;
                }

                for (int i = 0; i < SObjects.Count; i++)
                {
                    if (SObjects[i]._name == objectName)
                    {
                        save += SObjects[i].ToString(newLine, tabulation, 0);
                        break;
                    }
                }

                if (Path.GetFileName(path) == "")
                {
                    path = Path.Combine(path, objectName + extension);
                }
                else
                {
                    path = Path.ChangeExtension(path, extension);
                }

                File.WriteAllText(path, save);
            }

            /// <summry> Сохраняет все объекты в данную папку в один или несколько файлов.</summry>
            public void SaveAll(string path, bool hyphenation = false)
            {
                string tabulation = "", newLine = "", save = "";

                if (hyphenation)
                {
                    tabulation = "\t";
                    newLine = System.Environment.NewLine;
                }

                if (Path.GetFileName(path) == "")
                {   
                    for (int i = 0; i < SObjects.Count; i++)
                    {
                        save += SObjects[i].ToString(newLine, tabulation, 0);
                        File.WriteAllText(System.IO.Path.Combine(path, SObjects[i]._name + ".shai"), save);
                        save = "";
                    }
                }
                else
                {
                    for (int i = 0; i < SObjects.Count; i++)
                    {
                        save += SObjects[i].ToString(newLine, tabulation, 0);
                    }

                    path = Path.ChangeExtension(path, extension);

                    File.WriteAllText(path, save);
                }
            }

            public void Load(string path)
            {
                string str;

                if (Path.GetFileName(path) == "")
                {
                    string[] fileEntries = Directory.GetFiles(path);
                    foreach (string fileName in fileEntries)
                    {
                        if (Path.GetExtension(fileName) == extension)
                        {
                            str = File.ReadAllText(fileName);
                            ParseObjects(ref str);
                        }
                    }
                }
                else
                {
                    if (File.Exists(path))
                    {
                        str = File.ReadAllText(path);
                        ParseObjects(ref str);
                    }
                }  
            }

            private void ParseObjects(ref string str)
            {
                string name = "";
                bool isName = false, isValue = false;
                char sl;
                // for isObject
                bool objStart = false;
                int recess = 0; // углубление при чтении объекта (фигурные скобки)
                
                SObject parsingObject = null; // объект, который сейчас читается

                for (int i = 0; i < str.Length; ++i)
                {
                    if (isName)
                    {
                        if (str[i] == ' ' || str[i] == '\n' || str[i] == '\t' || str[i] == '\r')
                        {
                            isName = false;
                            continue;
                        }
                        if (str[i] == ':') // end enter name
                        {
                            for (int k = 0; k < SObjects.Count; ++k)
                            {
                                if (SObjects[k]._name == name)
                                {
                                    parsingObject = SObjects[k];
                                }
                            }

                            if (parsingObject == null)
                            {
                                parsingObject = new SObject(name);
                                SObjects.Add(parsingObject);
                            }

                            parsingObject.StartLoad();

                            isName = false;
                            isValue = true;
                            continue;
                        }
                        name += str[i];
                        continue;
                    }

                    if (isValue)
                    {
                        if (objStart)
                        {
                            if (str[i] == '{')
                            {
                                recess += 1;
                            }
                            else if (str[i] == '}')
                            {
                                recess -= 1;
                                if (recess == 0)
                                {
                                    parsingObject.EndLoad();
                                    parsingObject = null;
                                    objStart = false;
                                    isValue = false;
                                    name = "";
                                    continue;
                                }
                            }
                            sl = str[i];
                            parsingObject.Parse(ref sl);
                            continue;
                        }
                        else
                        {
                            if (str[i] == '{')
                            {
                                objStart = true;
                                recess = 1;
                                continue;
                            }
                            continue;
                        }
                    }

                    if (str[i] == ' ' || str[i] == '\n' || str[i] == '\t' || str[i] == '\r') continue;

                    if (str[i] == ':')
                    {
                        isValue = true;
                        continue;
                    }

                    if (name.Length > 0)
                    {
                        return;
                    }

                    isName = true;
                    name += str[i];
                }
                return;
            }

            private void CreateObject(List<string> ObjectsNames)
            {
                for (int i = 0; i < SObjects.Count; i++)
                {
                    if (SObjects[i]._name == ObjectsNames[0])
                    {
                        SObjects[i].AddObject(ref ObjectsNames, 1);
                        return;
                    }
                }
                SObjects.Add(new SObject(ObjectsNames[0]));
                SObjects[SObjects.Count - 1].AddObject(ref ObjectsNames, 1);
            }

            public void SetValue(string path, string name, int value)
            {
                MainSetValue(path, name, value.ToString());
            }

            public void SetValue(string path, string name, bool value)
            {
                MainSetValue(path, name, value.ToString());
            }

            public void SetValue(string path, string name, float value)
            {
                MainSetValue(path, name, value.ToString());
            }

            public void SetValue(string path, string name, string value)
            {
                MainSetValue(path, name, value);
            }

            public void SetValue<T>(string path, string name, T value)
            {
                MainSetValue(path, name, value.ToString());
            }

            private static string CheckSlash(string str)
            {
                string s = "";
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] == '\n')
                    {
                        s += @"\n";
                        continue;
                    }
                    if (str[i] == '{' || str[i] == '}' || str[i] == '"' || str[i] == '\\')
                    {
                        s += @"\";
                    }
                    s += str[i];
                }
                return s;
            }

            private static bool CheckToString(ref string str)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (str[i] == char.Parse(j.ToString()) || str[i] == '.' || str[i] == '-') break;
                        if (j >= 9)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            // set
            private void MainSetValue(string path, string name, string value)
            {
                List<string> ObjectsNames = PathToObjectsNames(ref path);
                CreateObject(ObjectsNames);

                for (int i = 0; i < SObjects.Count; i++)
                {
                    if (SObjects[i]._name == ObjectsNames[0])
                    {
                        SObjects[i].SetData(ref ObjectsNames, 1, ref name, ref value);
                        return;
                    }
                }
            }

            //get
            private string MainGetValue(string path, string name)
            {
                List<string> ObjectsNames = PathToObjectsNames(ref path);

                for (int i = 0; i < SObjects.Count; i++)
                {
                    if (SObjects[i]._name == ObjectsNames[0])
                    {
                        return SObjects[i].GetData(ref ObjectsNames, 1, ref name);
                    }
                }
                return null;
            }

            public string GetString(string path, string name)
            {
                return MainGetValue(path, name);
            }

            public int GetInt(string path, string name)
            {
                try
                {
                    int i = int.Parse(MainGetValue(path, name));
                    return i;
                }
                catch
                {
                    return 0;
                }
            }

            public float GetFloat(string path, string name)
            {
                try
                {
                    float i = float.Parse(MainGetValue(path, name));
                    return i;
                }
                catch
                {
                    return 0;
                }
            }

            public bool GetBool(string path, string name)
            {
                return MainGetValue(path, name) == "True";
            }

            public void RemoveObject(string path)
            {
                List<string> names = PathToObjectsNames(ref path);
                if (names.Count == 1)
                {
                    for (int i = 0; i < SObjects.Count; i++)
                    {
                        if (SObjects[i]._name == names[0])
                        {
                            SObjects[i].DeleteAllData();
                            SObjects.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < SObjects.Count; i++)
                    {
                        if (SObjects[i]._name == names[0])
                        {
                            SObjects[i].DeleteMyObject(ref names, 1);
                            SObjects.RemoveAt(i);
                        }
                    }
                }
            }

            public void Clear()
            {
                for (int i = 0; i < SObjects.Count; i++)
                {
                    SObjects[i].DeleteAllData();
                }
                SObjects.Clear();
            }

            private List<string> PathToObjectsNames(ref string path)
            {
                string str = "";
                List<string> ObjectNames = new List<string>();
                for (int i = 0; i < path.Length; ++i)
                {
                    if (path[i] == '.')
                    {
                        ObjectNames.Add(str);
                        str = "";
                        continue;
                    }
                    str += path[i];
                }
                ObjectNames.Add(str);
                return ObjectNames;
            }

            private class SObject
            {
                public string _name;
                public List<Data> _datas;
                public List<SObject> _objects;

                private VariablesForParseObject _vars;

                public SObject(string name)
                {
                    _name = name;
                    _datas = new List<Data>();
                    _objects = new List<SObject>();
                }

                public void AddData(Data data)
                {
                    for (int i = 0; i < _datas.Count; i++)
                    {
                        if (_datas[i].name == data.name)
                        {
                            _datas[i].value = data.value;
                            return;
                        }
                    }
                    _datas.Add(data);
                }

                public void AddObject(SObject obj, bool inheritance = false)
                {
                    if (obj._name == _name && inheritance) // наследование
                    {
                        for (int i = 0; i < obj._datas.Count; ++i)
                        {
                            AddData(obj._datas[i]);
                        }
                        for (int i = 0; i < obj._objects.Count; ++i)
                        {
                            AddObject(obj._objects[i]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < _objects.Count; ++i)
                        {
                            if (_objects[i]._name == obj._name)
                            {
                                _objects[i].AddObject(obj, true);
                                return;
                            }
                        }
                        _objects.Add(obj);
                    }
                }

                public void AddObject(ref List<string> ObjectsNames, int id)
                {
                    if (id + 1 == ObjectsNames.Count)
                    {
                        for (int i = 0; i < _objects.Count; i++)
                        {
                            if (_objects[i]._name == ObjectsNames[id])
                            {
                                return;
                            }
                        }
                        AddObject(new SObject(ObjectsNames[id]));
                    }
                    else if (id + 1 > ObjectsNames.Count)
                    {
                        return;
                    }
                    else
                    {
                        for (int i = 0; i < _objects.Count; i++)
                        {
                            if (_objects[i]._name == ObjectsNames[id])
                            {
                                _objects[i].AddObject(ref ObjectsNames, id + 1);
                                return;
                            }
                        }
                        SObject obj = new SObject(ObjectsNames[id]);
                        AddObject(obj);
                        obj.AddObject(ref ObjectsNames, id + 1);
                    }
                }

                public void DeleteMyObject(ref List<string> ObjectsNames, int id)
                {
                    if (id + 1 == ObjectsNames.Count)
                    {
                        for (int i = 0; i < _objects.Count; i++)
                        {
                            if (_objects[i]._name == ObjectsNames[id])
                            {
                                _objects[i].DeleteAllData();
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < _objects.Count; i++)
                        {
                            if (_objects[i]._name == ObjectsNames[id])
                            {
                                _objects[i].DeleteMyObject(ref ObjectsNames, id + 1);

                            }
                        }
                    }
                }

                public void DeleteAllData()
                {
                    _datas.Clear();
                    for (int i = 0; i < _objects.Count; i++)
                    {
                        _objects[i].DeleteAllData();
                    }
                    _objects.Clear();
                }

                public void SetData(ref List<string> ObjectsNames, int id, ref string name, ref string value)
                {
                    if (id == ObjectsNames.Count)
                    {
                        for (int i = 0; i < _datas.Count; i++)
                        {
                            if (_datas[i].name == name)
                            {
                                _datas[i].Set(name, value);
                                return;
                            }
                        }
                        AddData(new Data(name, value, CheckToString(ref value)));
                    }
                    else
                    {
                        for (int i = 0; i < _objects.Count; i++)
                        {
                            if (_objects[i]._name == ObjectsNames[id])
                            {
                                _objects[i].SetData(ref ObjectsNames, id + 1, ref name, ref value);
                                return;
                            }
                        }
                        SObject obj = new SObject(ObjectsNames[id]);
                        AddObject(obj, false);
                        obj.SetData(ref ObjectsNames, id + 1, ref name, ref value);
                    }
                }

                public string GetData(ref List<string> ObjectsNames, int id, ref string name)
                {
                    if (id == ObjectsNames.Count)
                    {
                        for (int i = 0; i < _datas.Count; i++)
                        {
                            if (_datas[i].name == name)
                            {

                                return _datas[i].value;
                            }
                        }
                        return null;
                    }
                    else
                    {
                        for (int i = 0; i < _objects.Count; i++)
                        {
                            if (_objects[i]._name == ObjectsNames[id])
                            {
                                return _objects[i].GetData(ref ObjectsNames, id + 1, ref name);
                            }
                        }
                        return null;
                    }
                }

                public string ToString(string newLine, string Tabulation, int CountTabulation)
                {
                    string str = "", tab = "";

                    for (int i = 0; i < CountTabulation; i++) tab += Tabulation;
                    str += tab + _name + ":" + newLine + tab + "{" + newLine;

                    for (int i = 0; i < _datas.Count; i++)
                    {
                        if (CheckToString(ref _datas[i].value))
                        {
                            str += tab + Tabulation + _datas[i].name + "=\"" + CheckSlash(_datas[i].value) + "\";" + newLine;
                        }
                        else
                        {
                            str += tab + Tabulation + _datas[i].name + "=" + _datas[i].value + ";" + newLine;
                        }
                    }
                    for (int i = 0; i < _objects.Count; i++)
                    {
                        str += _objects[i].ToString(newLine, Tabulation, CountTabulation + 1);
                    }
                    str += tab + "}" + newLine;
                    return str;
                }

                public void StartLoad()
                { 
                    _vars = new VariablesForParseObject();
                }

                public void EndLoad()
                { 
                    _vars = null; 
                }

                public void Parse(ref char str)
                {
                    if (_vars.isName)
                    {
                        if (str == ' ' || str == '\n' || str == '\t' || str == '\r')
                        {
                            return;
                        }
                        if (str == ':')
                        {
                            for (int k = 0; k < _objects.Count; ++k)
                            {
                                if (_objects[k]._name == _vars.name)
                                {
                                    _vars.parsingObject = _objects[k];
                                }
                            }
                            if (_vars.parsingObject == null)
                            {
                                _vars.parsingObject = new SObject(_vars.name);
                                _objects.Add(_vars.parsingObject);
                            }
                            _vars.parsingObject.StartLoad();

                            _vars.isName = false;
                            _vars.isValue = true;
                            _vars.isObject = true;
                            return;
                        }
                        if (str == '=')
                        {
                            _vars.isName = false;
                            _vars.isValue = true;
                            _vars.isVar = true;
                            return;
                        }
                        _vars.name += str;
                        return;
                    }

                    if (_vars.isValue)
                    {
                        if (_vars.isObject)
                        {
                            if (_vars.objStart)
                            {
                                if (str == '{')
                                {
                                    _vars.recess += 1;
                                }
                                else if (str == '}')
                                {
                                    _vars.recess -= 1;
                                    if (_vars.recess == 0)
                                    {
                                        _vars.parsingObject.EndLoad();
                                        _vars.parsingObject = null;
                                        _vars.objStart = false;
                                        _vars.isValue = false;
                                        _vars.name = "";
                                        return;
                                    }
                                }
                                _vars.parsingObject.Parse(ref str);
                                return;
                            }
                            else
                            {
                                if (str == '{')
                                {
                                    _vars.objStart = true;
                                    _vars.recess = 1;
                                    return;
                                }
                                return;
                            }
                        }

                        if (_vars.isVar)
                        {
                            if (str == ' ' && !_vars.isString) return;
                            if (str == '"')
                            {
                                _vars.glString = true;
                                if (_vars.isString) // уже строка началась?
                                {
                                    _vars.isString = false;
                                    return;
                                }
                                _vars.isString = true;
                                return;
                            }
                            if (str == '\\') { _vars.isScan = true; return; }
                            if (_vars.isScan) { _vars.value += '\\' + str; _vars.isScan = false; return; }
                            if (str == ';')
                            {
                                _vars.isVar = false;
                                _vars.isValue = false;
                                _vars.isString = false;
                                AddData(new Data(_vars.name, _vars.value, _vars.glString));
                                _vars.glString = false;
                                _vars.name = "";
                                _vars.value = "";
                                return;
                            }
                            if (str == '\r' || str == '\n' || str == '\t' || str == '\r') return;
                            _vars.value += str;
                            return;
                        }
                    }

                    if (str == ' ' || str == '\n' || str == '\t' || str == '\r') return;

                    if (str == '=')
                    {
                        _vars.isVar = true;
                        _vars.isValue = true;
                        return;
                    }

                    if (str == ':')
                    {
                        _vars.isObject = true;
                        _vars.isValue = true;
                        return;
                    }

                    if (_vars.name.Length > 0)
                    {
                        throw new System.Exception("Пропущено присвоение (=,:) " + str);
                    }

                    _vars.isName = true;
                    _vars.name += str;
                }

                public class Data
                {
                    public string name;
                    public string value;

                    public Data(string name, string value, bool isString)
                    {
                        this.name = name;
                        if (isString)
                        {
                            string v = "";
                            for (int i = 0; i < value.Length; i++)
                            {
                                if (value[i] == '\\')
                                {
                                    if (value[++i] == 'n')
                                    {
                                        v += "\n";
                                    }
                                    else
                                    {
                                        v += value[i];
                                    }
                                    continue;
                                }
                                v += value[i];
                            }
                            this.value = v;
                        }
                        else
                        {
                            this.value = value;
                        }
                    }

                    public void Set(string name, string value)
                    {
                        this.name = name;
                        this.value = value;
                    }
                }

                private class VariablesForParseObject
                {
                    public string name = "", value = "";
                    public bool isName = false, isObject = false, isVar = false, isValue = false;
                    // for isObject
                    public bool objStart = false;
                    public int recess = 1;
                    //
                    // for isVar
                    public bool isString = false;
                    public bool glString = false;
                    public bool isScan = false;
                    //
                    public SObject parsingObject;

                    public VariablesForParseObject()
                    {
                        name = "";
                        value = "";
                        isName = false;
                        isObject = false;
                        isValue = false;
                        isVar = false;
                        objStart = false;
                        isScan = false;
                        isString = false;
                        glString = false;
                        recess = 1;
                    }
                }
            }
        }
    }
}