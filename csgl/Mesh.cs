using System;
using System.Collections.Generic;
using OpenTK;

/*
 * Один Mesh может принадлежать разным объектам, которые имеют разные материалы
*/

namespace csgl {
  public class Mesh: IDisposable, IResourceEntity {
    public Resource resource;
    private List< Vector3 > _vertices;
    private Queue< Vector4 > _colors;
    private Queue< Vector2 > _texCoords;
    private Queue< Vector3 > _normals;
    private Queue< UInt32 > _indices;
    private string _name;
    private int _hash;

    public List< Vector3 > vertices {
      get { return this._vertices; }
      private set { this._vertices = value; }
    }

    public Queue< Vector4 > colors {
      get { return this._colors; }
      private set { this._colors = value; }
    }

    public Queue< Vector2 > texCoords {
      get { return this._texCoords; }
      private set { this._texCoords = value; }
    }

    public Queue< Vector3 > normals {
      get { return this._normals; }
      private set { this._normals = value; }
    }

    public Queue< UInt32 > indices {
      get { return this._indices; }
      private set { this._indices = value; }
    }

    public int hash {
      get { return this._hash; }
      private set { this._hash = value; }
    }

    private Mesh() {
    }

    public string name {
      get{ return this._name; }
      private set{ this._name = value; }
    }

    public Mesh( string setName, string source ) {
      this.name = setName;
      this.vertices = new List< Vector3 >();
      this.colors = new Queue< Vector4 >();
      this.texCoords = new Queue< Vector2 >();
      this.normals = new Queue< Vector3 >();
      this.indices = new Queue< UInt32 >();
      this.hash = this.name.GetHashCode();
      foreach( string line in source.Replace( '.', ',' ).Split( '\n' ) ) {
        if( line.Length < 5 ) { //5 = '0 0 0'
          continue;
        }
        if( line[ 0 ] == 'i' ) { //index
          string [] indexList = line.Substring( 2 ).Split( ' ' );
          int indexNum = 0;
          Vector3 [] polygon = {
            Vector3.Zero,
            Vector3.Zero,
            Vector3.Zero,
          };
          //Vector3 normal;
          foreach( string indexStr in indexList ) {
            UInt32 index = UInt32.Parse( indexStr );
            this.indices.Enqueue( index );
            /* polygon[ 0 ] = polygon[ 1 ];
            polygon[ 1 ] = polygon[ 2 ];
            polygon[ 2 ] = this.vertices.ToArray()[ index ];
            normal = Vector3.Cross( polygon[ 2 ] - polygon[ 0 ], polygon[ 1 ] - polygon[ 0 ] ).Normalized();
            Console.WriteLine( "=> normal {0}", normal );
            this.normals.Enqueue( normal ); */
            if( indexNum == 0 || indexNum == indexList.Length - 1 ) { //first and last indices doubles
              this.indices.Enqueue( index );
              /* this.normals.Enqueue( normal );
              Console.WriteLine( "=> normal {0}", normal ); */
            }
            ++indexNum;
          }
        } else { //vertice
          string [] floatList = line.Split( ' ' );
          if( floatList.Length < 3 ) {
            Console.WriteLine( "[ERROR] Mesh '{0}' has bad vertice information: '{1}'", this.name, line );
          } else {
            this.vertices.Add( new Vector3( float.Parse( floatList[ 0 ] ), float.Parse( floatList[ 1 ] ), float.Parse( floatList[ 2 ] ) ) );
            if( floatList.Length >= 7 ) { //color
              this.colors.Enqueue( new Vector4( float.Parse( floatList[ 3 ] ), float.Parse( floatList[ 4 ] ), float.Parse( floatList[ 5 ] ), float.Parse( floatList[ 6 ] ) ) );
              if( floatList.Length >= 9 ) { //texCoords
                this.texCoords.Enqueue( new Vector2( float.Parse( floatList[ 7 ] ), float.Parse( floatList[ 8 ] ) ) );
                if( floatList.Length >= 11 ) { //normal
                  this.normals.Enqueue( new Vector3( float.Parse( floatList[ 9 ] ), float.Parse( floatList[ 10 ] ), float.Parse( floatList[ 11 ] ) ) );
                } else {
                  this.normals.Enqueue( Vector3.Zero );
                }
              } else {
                this.texCoords.Enqueue( Vector2.Zero );
              }
            } else {
              this.colors.Enqueue( Vector4.One );
            }
          }
        }
      }
    }

    ~Mesh() {
      this.Dispose( true );
    }

    public void Dispose() {
      this.Dispose( true );
      GC.SuppressFinalize( this );
    }

    public void Inc() {
      if( this.resource != null ) {
        this.resource.Inc();
      }
    }

    public void Dec() {
      if( this.resource != null ) {
        this.resource.Dec();
      }
    }

    protected virtual void Dispose( bool disposing ) {
      if( disposing ) {
        if( this.vertices != null ) {
          Console.WriteLine( "~Mesh {0}", this.name );
          this._vertices = null;
          this._colors = null;
          this._texCoords = null;
          this._normals = null;
          this._indices = null;
          if( this.resource != null ) {
            this.resource.Dispose();
            this.resource = null;
          }
          Console.WriteLine( "~Mesh {0} done", this.name );
        }
      }
    }
  }
}

