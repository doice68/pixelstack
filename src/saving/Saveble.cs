using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

public class Saveble
{
    //TODO: this sucks try to get reflection implemention working it's wayy better. 
    public static Settings settings = new();
    Dictionary<string, object> s = new();

    public Saveble()
    {
        App.instance.onSave += OnSave;
    }
    public virtual void OnLoad(){}
    public virtual void OnSave(){}

    #region reflection implemention
    // public class SaveField : Attribute
    // {
    //     public string name;
    //     public SaveField([CallerMemberName] string SerializationName = null)
    //     {
    //         name = SerializationName;
    //     }
    // }
    // public virtual void Load()
    // {
    //     if (File.Exists(this.GetType().ToString()) == false) 
    //         return;

    //     Console.WriteLine("loading " + this.GetType().ToString());
        
    //     var jsonText = File.ReadAllText(this.GetType().ToString());
    //     var desreializedData = JsonConvert.DeserializeObject<List<dynamic>>(jsonText);

    //     foreach (var field in desreializedData)
    //     {
    //         var f = this.GetType().GetField((string)field.fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
    //         if (f != null)
    //         {
    //             Console.WriteLine((string)field.fieldName);
    //             var t = (Type)field.vType;
    //
    //             f.SetValue(this, Convert.ChangeType(field.value, (Type)field.vType));
    //             
    //         }
    //     }
    // }
    
    // public virtual void Save()
    // {
    //     Console.WriteLine("saving " + this.ToString());
    //     List<dynamic> data = new List<dynamic>(){};
        
    //     FieldInfo[] fields = this.GetType()
    //         .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
    //         .Where(f => f.GetCustomAttribute<CompilerGeneratedAttribute>() == null)
    //         .Where(f => f.GetCustomAttribute<SaveField>() != null)
    //         .ToArray();

    //     foreach (var field in fields)
    //     {
    //         var attr = field.GetCustomAttribute<SaveField>();
    //         var fieldName = field.Name;
    //         data.Add(new {fieldName, value = field.GetValue(this), vType = field.GetValue(this).GetType()});
    //     }
    //     var jsonText = JsonConvert.SerializeObject(data, Formatting.Indented);
    //     File.WriteAllTextAsync(this.GetType().ToString(), jsonText);
    // }
    #endregion
}