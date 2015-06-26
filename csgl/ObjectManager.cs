﻿using System;
using System.Collections.Generic;

namespace csgl {
  public class ObjectManager: IDisposable {
    private bool _isValid = false;
    private List< Object > objectList;

    public bool isValid {
      get { return this._isValid; }
      private set { this._isValid = value; }
    }

    public ObjectManager() {
      this.objectList = new List< Object >();
      this.isValid = true;
    }

    ~ObjectManager() {
      this.Dispose( true );
    }

    public Object CreateObject( string name ) {
      Object obj = new Object( name );
      this.objectList.Add( obj );
      return obj;
    }

    public void Render() {
      foreach( var obj in this.objectList ) {
        obj.Render();
      }
    }

    public void Dispose() {
      this.Dispose( true );
      GC.SuppressFinalize( this );
    }

    protected virtual void Dispose( bool disposing ) {
      if( disposing ) {
        if( this.isValid ) {
          Console.WriteLine( "~ObjectManager" );
          this.isValid = false;
          foreach( var obj in this.objectList ) {
            obj.Dispose();
          }
          this.objectList = null;
          Console.WriteLine( "~ObjectManager done" );
        }
      }
    }
  }
}

