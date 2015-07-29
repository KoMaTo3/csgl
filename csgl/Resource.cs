using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL;
using GLib;

namespace csgl {
  public class Resource: IDisposable {
    protected Int64 pointers = 0;

    public string name {
      get { return this._name; }
      protected set { this._name = value; }
    }

    public ResourceType type {
      get { return this._type; }
      protected set { this._type = value; }
    }

    public bool IsValid {
      get { return this.isValid; }
      protected set { this.isValid = value; }
    }

    private static List< Resource > resourceList = new List< Resource >();
    private static UInt64 _lastId = 0;
    private ResourceType _type = ResourceType.UNKNOWN;
    private string _name;
    private bool isValid = false;
    private UInt64 _id;

    protected UInt64 id {
      get { return this._id; }
      private set { this._id = value; }
    }

    private Resource() {
    }

    public Resource( string setName ) {
      this.name = setName;
    }

    ~Resource() {
      this.Dispose( true );
    }

    public void Dispose() {
      this.Dispose( true );
      GC.SuppressFinalize( this );
    }

    protected virtual void Dispose( bool disposing ) {
      if( disposing ) {
        if( this.isValid ) {
          Console.WriteLine( "~Resource" );
          Resource.RemoveResource( this.id );
          this.isValid = false;
          Console.WriteLine( "~Resource done" );
        }
      }
    }

    protected static void RemoveResource( UInt64 id ) {
      foreach( Resource res in Resource.resourceList ) {
        if( res.id == id ) {
          Resource.resourceList.Remove( res );
          Console.WriteLine( "-resource {0}: count[{1}]", id, Resource.resourceList.Count );
          break;
        }
      }
    }

    public static void DisposeAll() {
      while( Resource.resourceList.Count > 0 ) {
        Resource res = Resource.resourceList[ 0 ];
        res.Dispose();
      }
      Resource.resourceList.Clear();
    }

    public void Dec() {
      if( this.pointers == 0 ) {
        Console.WriteLine( "[ERROR] Resource.Dec() => pointers already 0" );
      } else {
        --this.pointers;
      }
    }

    public void Inc() {
      ++this.pointers;
    }

    public static Resource Get( string fileName ) {
      foreach( var res in Resource.resourceList ) {
        if( res.name == fileName ) {
          Console.WriteLine( "Resource: getted old resource '{0}'", res );
          return res;
        }
      }

      Resource result = null;

      byte [] data;
      try {
        data = File.ReadAllBytes( fileName );
      } catch( Exception ) {
        Console.WriteLine( "[ERROR] Resource.Get - failed to load file '{0}'", fileName );
        return result;
      }

      string extension = Path.GetExtension( fileName ).ToLower();
      Console.WriteLine( "Resource: {0}", extension );
      switch( extension ) {
        case ".vs":
          {
            result = new ResourceShader( fileName, System.Text.Encoding.Default.GetString( data ), ShaderType.VertexShader );
          }
        break; //vs
        case ".fs":
          {
            result = new ResourceShader( fileName, System.Text.Encoding.Default.GetString( data ), ShaderType.FragmentShader );
          }
        break; //fs
        case ".sp":
          {
            result = new ResourceShaderProgram( fileName, System.Text.Encoding.Default.GetString( data ) );
            Console.WriteLine( "ResourceShaderProgram loaded" );
          }
        break; //sp
        case ".tga":
          {
            result = new ResourceImage( fileName, data );
          }
        break; //tga
        case ".tex":
          {
            result = new ResourceTexture( fileName, System.Text.Encoding.Default.GetString( data ) );
          }
        break; //tex
        case ".mat":
          {
            result = new ResourceMaterial( fileName, System.Text.Encoding.Default.GetString( data ) );
          }
        break; //mat
        case ".mesh":
          {
            result = new ResourceMesh( fileName, System.Text.Encoding.Default.GetString( data ) );
          }
        break; //mesh
        case ".model":
          {
            result = new ResourceModel( fileName, System.Text.Encoding.Default.GetString( data ) );
          }
        break; //model
      }//switch extension

      if( result != null ) {
        result.name = fileName;
        result.id = Resource._lastId++;
        Resource.resourceList.Add( result );
        Console.WriteLine( "+resource: {0}", Resource.resourceList.Count );
      }

      return result;
    }

    public static List< Resource > GetAllOfType( ResourceType type ) {
      var resultList = new List< Resource >();
      foreach( Resource res in Resource.resourceList ) {
        if( res.type == type ) {
          resultList.Add( res );
        }
      }
      return resultList;
    }

    public static List< ShaderProgram > GetAllShaderProgram() {
      var resourceList = Resource.GetAllOfType( ResourceType.SHADER_PROGRAM );
      var shadersList = new List< ShaderProgram >();
      foreach( var res in resourceList ) {
        shadersList.Add( ( ShaderProgram ) res );
      }
      return shadersList;
    }

    public static explicit operator Material( Resource resource ) {
      return ( resource.type == ResourceType.MATERIAL ? ( Material ) ( ResourceMaterial ) resource : null );
    }

    public static explicit operator Shader( Resource resource ) {
      return ( resource.type == ResourceType.SHADER ? ( Shader ) ( ResourceShader ) resource : null );
    }

    public static explicit operator ShaderProgram( Resource resource ) {
      return ( resource.type == ResourceType.SHADER_PROGRAM ? ( ShaderProgram ) ( ResourceShaderProgram ) resource : null );
    }

    public static explicit operator Texture( Resource resource ) {
      return ( resource.type == ResourceType.TEXTURE ? ( Texture ) ( ResourceTexture ) resource : null );
    }

    public static explicit operator Mesh( Resource resource ) {
      return ( resource.type == ResourceType.MESH ? ( Mesh ) ( ResourceMesh ) resource : null );
    }

    public static explicit operator Model( Resource resource ) {
      return ( resource.type == ResourceType.MODEL ? ( Model ) ( ResourceModel ) resource : null );
    }

    /* public static explicit operator string( Resource resource ) {
      return string.Format( "{0}:{1}", resource.name, resource.type );
    } */

    public override String ToString() {
      return string.Format( "{0}:{1}", this.name, this.type );
    }
  }
}

