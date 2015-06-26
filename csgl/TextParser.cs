using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace csgl {
  public class TextParser {
    Regex expression;

    public TextParser() {
      this.expression = new Regex( @"\s*(\w+)\s*=\s*(((\w+))|(""([^""]+)"")|('([^']+)'))\s*" );
    }

    public Dictionary< string, string > ParseLine( string str ) {
      Match match = this.expression.Match( str );
      if( !match.Success ) {
        return null;
      }

      string name, value;
      var parseResult = new Dictionary< string, string >();
      GroupCollection groups = match.Groups;
      name = groups[ 1 ].Value;
      value = groups[ 4 ].Value ?? groups[ 6 ].Value ?? groups[ 8 ].Value;
      parseResult.Add( name, value );

      return parseResult;
    }

    public Dictionary< string, string > ParseText( string str ) {
      MatchCollection result = this.expression.Matches( str );
      if( result.Count < 1 ) {
        return null;
      }

      string name, value = "";
      var parseResult = new Dictionary< string, string >();
      Console.WriteLine( "matches: {0}", result.Count );
      foreach( Match match in result ) {
        GroupCollection groups = match.Groups;
        name = groups[ 1 ].Value;
        this.SelectValue( groups, ref value );
        parseResult.Add( name, value );
      }

      return parseResult;
    }

    private int SelectValue( GroupCollection groups, ref string str ) {
      int q = 4;
      do {
        str = groups[ q ].Value;
        q += 2;
      } while( q <= 8 && str.Length == 0 );
      return q - 2;
    }

    ~TextParser() {
    }
  }
}

