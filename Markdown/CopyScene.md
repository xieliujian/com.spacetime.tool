# 复制场景工具

## 说明

![GitHub](https://github.com/xieliujian/UnityDemo_CopyScene/blob/main/Video/1.png?raw=true)

复制场景，例如把场景从10001复制到10002, 对于场景文件夹内的内容，为了避免引用旧资源，需要用代码复制

## 细节

``` c#

// 把meta文件内的GUID替换掉

static void MetaFileReWrite(FileInfo fileinfo)
{
    var text = File.ReadAllText(fileinfo.path);

    foreach (var dependfile in fileinfo.dependList)
    {
        var assetSrcFile = EditorUtils.ABSPath2AssetsPath(dependfile);
        var srcguid = AssetDatabase.AssetPathToGUID(assetSrcFile);

        var dstfile = ConvertResSrcPath2DstPath(dependfile);
        if (!File.Exists(dstfile))
        {
            Debug.Log("[CopyScene][UnReWrite] : " + fileinfo.path);
            continue;
        }

        var assetDstFile = EditorUtils.ABSPath2AssetsPath(dstfile);
        var dstguid = AssetDatabase.AssetPathToGUID(assetDstFile);
        text = text.Replace(srcguid, dstguid);
    }

    File.WriteAllText(fileinfo.path, text);
}

```

