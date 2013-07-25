using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WEHTiled {
    private List<WEHLayer> _layers;
    public List<WEHTileset> _tileset;
    
    public WEHTiled(string fileName) {
        _layers = new List<WEHLayer>();
        _tileset = new List<WEHTileset>();
        
        TextAsset jsonString = (TextAsset)Resources.Load(fileName, typeof(TextAsset));
        Dictionary<string,object> tiledJson = jsonString.text.dictionaryFromJson();
        
        object tilesets;
        if(!tiledJson.TryGetValue("tilesets", out tilesets)) {
            throw new FutileException("No associated tileset found!");
        }
        foreach(object tilesetJson in tilesets as List<object>)
            _tileset.Add(new WEHTileset(tilesetJson as Dictionary<string, object>));
        
        object layers;
        if(!tiledJson.TryGetValue("layers", out layers)) {
            throw new FutileException("No associated tileset found!");
        }
        foreach(object layerJson in layers as List<object>)
            _layers.Add(new WEHLayer(layerJson as Dictionary<string, object>));
    }
    public FContainer DrawLayer(int id){
        FContainer cont = new FContainer();
        for(int row = 0;row < _layers[id]._height; row++) {
            for(int column = 0;column < _layers[id]._width; column++) {
                int sid = _layers[id]._data[(row*_layers[id]._width)+column];
                if(sid!=0){
                    FSprite s = new FSprite(_tileset[0]._elements[sid-1]);
                    s.x = column*32;
                    s.y = -row*32;
                    cont.AddChild(s);
                }
            }
        }
        return cont;
    }
}

public class WEHLayer {
    public List<int> _data;
    public int _height;
    public int _width;
    
    public WEHLayer(IDictionary layerJson) {
        _height = int.Parse(layerJson["height"].ToString());
        _width = int.Parse(layerJson["width"].ToString());
        
        _data = (layerJson["data"] as List<object>).ConvertAll<int>(x => int.Parse(x.ToString()));
    }
}

public class WEHTileset {
    
    int _tileHeight;
    int _tileWidth;
    
    FAtlas _atlas;
    public List<FAtlasElement> _elements;

    public WEHTileset(IDictionary tilesetJson) {
        _tileHeight = int.Parse(tilesetJson["tileheight"].ToString());
        _tileWidth = int.Parse(tilesetJson["tilewidth"].ToString());
        
        Futile.atlasManager.ActuallyLoadAtlasOrImage(tilesetJson["name"].ToString(),tilesetJson["name"].ToString(),"");
        _atlas = Futile.atlasManager.GetAtlasWithName(tilesetJson["name"].ToString());
        _elements = new List<FAtlasElement>();
        LoadElements();
    }
    
    private void LoadElements() {
        float scaleInverse = Futile.resourceScaleInverse;
        
        int index = 0;
        
        int rows = _atlas.texture.height / this._tileHeight;
        int columns = _atlas.texture.width / this._tileWidth;
        
        for(int row = 0;row < rows; row++) {
            for(int column = 0;column < columns; column++) {
                FAtlasElement element = new FAtlasElement();
        
                element.indexInAtlas = index++;
        
                element.name = element.indexInAtlas.ToString();
        
                element.isTrimmed = false;
        
                float frameX = (float)column*this._tileWidth;
                float frameY = (float)row*this._tileHeight;
                float frameW = (float)this._tileWidth;
                float frameH = (float)this._tileHeight; 
        
                Rect uvRect = new Rect
            (
             frameX / _atlas.textureSize.x,
             ((_atlas.textureSize.y - frameY - frameH) / _atlas.textureSize.y),
             frameW / _atlas.textureSize.x,
             frameH / _atlas.textureSize.y
            );
         
                element.uvRect = uvRect;
        
                element.uvTopLeft.Set(uvRect.xMin, uvRect.yMax);
                element.uvTopRight.Set(uvRect.xMax, uvRect.yMax);
                element.uvBottomRight.Set(uvRect.xMax, uvRect.yMin);
                element.uvBottomLeft.Set(uvRect.xMin, uvRect.yMin);
        
        
                element.sourcePixelSize.x = (float)this._tileWidth;
                element.sourcePixelSize.y = (float)this._tileHeight; 
        
                element.sourceSize.x = element.sourcePixelSize.x * scaleInverse;    
                element.sourceSize.y = element.sourcePixelSize.y * scaleInverse;
        
        
                float rectX = 0 * scaleInverse;
                float rectY = 0 * scaleInverse;
                float rectW = (float)this._tileWidth * scaleInverse;
                float rectH = (float)this._tileHeight * scaleInverse;
        
                element.sourceRect = new Rect(rectX, rectY, rectW, rectH);
                element.atlas = _atlas;
                element.atlasIndex = _atlas.index;
                
                _elements.Add(element);
            }
        }
        
    }
}
