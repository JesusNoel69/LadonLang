using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LadonLang.Data;

namespace LadonLang.Semantic
{
    public enum SymbolKind
    {
        VARIABLE,
        FUNCTION,
        NAMMEDSENTNCE
    }
    public record TypeRef(
        string Name,                 // "int", "string", "MyClass"
        double? Arg1 = null,         // DataTypeNode size/arg
        double? Arg2 = null
    );

    public abstract record SymbolBase(
        SymbolKind Kind,
        string Name,
        Token DeclToken
    );

    public record VariableSymbol(
        string Name,
        Token DeclToken,
        IReadOnlyList<TypeRef> AllowedTypes, // TypeArguments() -> union
        bool IsOutParam = false,
        bool IsParameter = false,
        bool IsAssignedFromInputSet = false
    ) : SymbolBase(SymbolKind.VARIABLE, Name, DeclToken);

    public record FunctionSymbol(
        string Name,
        Token DeclToken,
        IReadOnlyList<VariableSymbol> Parameters,
        VariableSymbol? OutParameter,
        IReadOnlyList<TypeRef> ReturnTypes,
        int? OutPosition
    ) : SymbolBase(SymbolKind.FUNCTION, Name, DeclToken);

    public record NamedSentenceSymbol(
        string Name,
        Token DeclToken
    ) : SymbolBase(SymbolKind.NAMMEDSENTNCE, Name, DeclToken);

    public sealed class Scope
    {
        public string ScopeName { get; }
        public Scope? Parent { get; }
        private readonly Dictionary<string, SymbolBase> _symbols = new();
        public Scope(string scopeName, Scope? parent)
        {
            ScopeName = scopeName;
            Parent = parent;
        }
        public bool TryDeclare(SymbolBase sym, out ParserExeption? error)
        {
            error = null;
            if (_symbols.ContainsKey(sym.Name))
            {
                // maybe needs a new exception (DuplicateSymbolException)
                error = new ParserExeption($"Duplicated symbol: '{sym.Name}'", sym.DeclToken);
                return false;
            }
            _symbols.Add(sym.Name, sym);
            return true;
        }
        public SymbolBase? Resolve(string name)
        {
            for (Scope? s = this; s != null; s = s.Parent)
                if (s._symbols.TryGetValue(name, out var sym)) return sym;

            return null;
        }
        public IEnumerable<SymbolBase> Symbols => _symbols.Values;
    }

    public sealed class SymbolTable
    {
        public Scope Current { get; private set; }

        public SymbolTable()
        {
            Current = new Scope("global", null);
        }
        public void Push(string name) => Current = new Scope(name, Current);
        public void Pop() => Current = Current.Parent ?? Current;
    }
}