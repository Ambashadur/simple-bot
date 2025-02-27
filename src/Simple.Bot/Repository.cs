using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Simple.Bot;

internal sealed class Repository
{
    public static void Initialize(string path) {
        using var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);

        try {
            var values = JsonSerializer.Deserialize<List<object>>(stream);
        } catch {
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(Encoding.Default.GetBytes("[]"));
        }
    }

    public static TEntity[] Load<TEntity>(string path) where TEntity : class {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        var values = JsonSerializer.Deserialize<TEntity[]>(stream);
        return values ?? Array.Empty<TEntity>();
    }

    public static void Store<TEntity>(string path, TEntity entity) where TEntity : class {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
        var values = JsonSerializer.Deserialize<List<TEntity>>(stream);
        values ??= new List<TEntity>();

        values.Add(entity);

        stream.Seek(0, SeekOrigin.Begin);

        JsonSerializer.Serialize(stream, values);
    }
}
