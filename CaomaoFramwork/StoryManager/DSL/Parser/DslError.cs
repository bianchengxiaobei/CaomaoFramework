using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptableData.Parser
{
#if UNITY_EDITOR
  class DslError
  {
    internal DslError(DslToken tokens)
    {
      this.tokens = tokens;
    }

    internal bool HasError
    {
      get { return mHasError; }
    }

    internal short mismatch(short terminal, short token)
    {
      mHasError = true;
      //LogSystem.Error(" expecting {0} but found {1}", DslString.GetSymbolName(terminal), DslString.GetSymbolName(token));
      return token;
    }

    internal short no_entry(short nonterminal, short token, int level)
    {
      mHasError = true;
      //LogSystem.Error(" syntax error: skipping input {0}", DslString.GetSymbolName(token));
      token = tokens.get(); // advance the input
      return token;
    }

    internal void input_left()
    {
      //LogSystem.Error("input left.");
    }

    internal void message(string message)
    {
      //LogSystem.Error("{0}", message);
    }

    private DslToken tokens;
    private bool mHasError;
  }
#endif
}
