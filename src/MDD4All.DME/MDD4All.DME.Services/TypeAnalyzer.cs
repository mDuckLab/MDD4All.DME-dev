using System;
using System.Collections.Generic;

namespace MDD4All.DME.Analyzers
{
    public enum TypeCategory
    {
        /// <summary>
        /// The initial state before analysis, or the state if 'null' was passed as the type to analyze.
        /// </summary>
        Null = 0,

        /// <summary>
        /// A complex type that is not a collection (e.g., a standard class).
        /// </summary>
        None = 10,

        /// <summary>
        /// A simple data type (e.g., int, string, bool, decimal, DateTime, Enum).
        /// </summary>
        Simple = 20,

        /// <summary>
        /// A simple data type that is nullable (e.g., int?, bool?).
        /// </summary>
        SimpleNullable = 21,

        /// <summary>
        /// A standard array (e.g., string[]).
        /// </summary>
        Array = 35,

        /// <summary>
        /// Represents a type that implements IList&lt;T&gt;.
        /// IList&lt;T&gt; inherits from ICollection&lt;T&gt; and IEnumerable&lt;T&gt; (enables 'foreach').
        /// Key implementations: List&lt;T&gt;, ObservableCollection&lt;T&gt;, Collection&lt;T&gt;.
        /// </summary>
        IList = 40,

        /// <summary>
        /// Represents a type that implements IDictionary&lt;TKey, TValue&gt;.
        /// IDictionary&lt;TKey, TValue&gt; inherits from IEnumerable&lt;KeyValuePair&lt;TKey, TValue&gt;&gt; (enables 'foreach').
        /// Key implementations: Dictionary&lt;TKey, TValue&gt;, SortedDictionary&lt;TKey, TValue&gt;, ConcurrentDictionary&lt;TKey, TValue&gt;.
        /// </summary>
        IDictionary = 45,

        /// <summary>
        /// Another generic type that is neither an IList nor an IDictionary (e.g., HashSet&lt;T&gt;, Queue&lt;T&gt;).
        /// </summary>
        OtherGenericType = 80,

        /// <summary>
        /// An error occurred during analysis.
        /// </summary>
        Error = 90
    }

    public class TypeAnalyzer
    {
        #region constructor and Factory Methods (Static)
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeAnalyzer"/> class.
        /// </summary>
        public TypeAnalyzer()
        {
            this.UnderlyingTypes = new List<Type>();
            this.AnalyzeType = null;
            TypeCategory = TypeCategory.Null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeAnalyzer"/> class by copying the state
        /// from an existing analyst instance.
        /// </summary>
        /// <param name="other">The source <see cref="TypeAnalyzer"/> instance to copy from.</param>
        /// <remarks>
        /// This constructor performs a deep copy of the <see cref="UnderlyingTypes"/> collection 
        /// using a foreach loop. This ensures that the new analyst has its own independent list 
        /// of types, preventing unintended side effects when modifying child nodes.
        /// </remarks>
        public TypeAnalyzer(TypeAnalyzer other)
        {
            this.UnderlyingTypes = new List<Type>();

            if (other != null)
            {
                this._analyzeType = other.AnalyzeType;
                this.TypeCategory = other.TypeCategory;
                foreach (Type type in other.UnderlyingTypes)
                {
                    this.UnderlyingTypes.Add(type);
                }
            }
        }

        /// <summary>
        /// Creates and analyzes a new <see cref="TypeAnalyzer"/> instance based on the provided <see cref="Type"/>.
        /// </summary>
        /// <param name="AnalyzeType">The <see cref="Type"/> to analyze.</param>
        /// <returns>A new, analyzed <see cref="TypeAnalyzer"/> instance.</returns>
        public static TypeAnalyzer CreateAnalyst(Type AnalyzeType)
        {
            TypeAnalyzer result = new TypeAnalyzer();
            result.Analyze(AnalyzeType);
            return result;
        }

        /// <summary>
        /// Creates and analyzes a new <see cref="TypeAnalyzer"/> instance based on the static (compile-time) type of the provided object.
        /// </summary>
        /// <typeparam name="T">The static type of the object to analyze.</typeparam>
        /// <param name="analyzeObject">The object to analyze.</param>
        /// <returns>A new, analyzed <see cref="TypeAnalyzer"/> instance.</returns>
        public static TypeAnalyzer CreateAnalyst<T>(T analyzeObject)
        {
            // <T> (Generics) are used instead of 'object' to correctly handle 'null' inputs.
            // Using 'object obj' and calling 'obj.GetType()' would throw a NullReferenceException
            // if the user passed in 'null'.
            // <T> avoids this by using 'typeof(T)', which gets the STATIC (compile-time)
            // type of the variable, which is known even if the value is 'null'.
            TypeAnalyzer result = new TypeAnalyzer();
            result.Analyze(analyzeObject);
            return result;
        }
        #endregion

        #region properties
        private Type? _analyzeType;

        /// <summary>
        /// The <see cref="Type"/> that is currently being analyzed.
        /// </summary>
        public Type? AnalyzeType
        {
            get
            {
                return _analyzeType;
            }
            private set
            {
                this._analyzeType = value;
                this.RefreshTypeDataCollection();
            }
        }

        /// <summary>
        /// The resulting classification category (e.g., Simple, IList) 
        /// determined by the analysis of the <see cref="AnalyzeType"/>.
        /// </summary>
        public TypeCategory TypeCategory { get; private set; }

        /// <summary>
        /// Gets a list of underlying types found during analysis.
        /// <br/>- For <c>Simple</c>: The simple type itself.
        /// <br/>- For <c>SimpleNullable</c>: The base value type (e.g., 'int' for 'int?').
        /// <br/>- For <c>Array</c>: The element type (e.g., 'string' for 'string[]').
        /// <br/>- For <c>IList</c>: The generic argument (e.g., 'string' for 'List&lt;string&gt;').
        /// <br/>- For <c>IDictionary</c>: The key type (index 0) and value type (index 1).
        /// <br/>- For <c>OtherGenericType</c>: All generic arguments.
        /// </summary>
        public List<Type> UnderlyingTypes { get; private set; }
        #endregion

        #region input Functions

        /// <summary>
        /// Analyzes the static (compile-time) type of the provided object.
        /// </summary>
        /// <typeparam name="T">The static type of the object.</typeparam>
        /// <param name="analyzeObject">The object to analyze.</param>
        public void Analyze<T>(T analyzeObject)
        {
            if (analyzeObject != null)
            {
                // 1. If the object exists, we look at what it REALLY is (Runtime Type).
                // Example: Variable is 'object', but content is 'Person' -> returns 'Person'.
                this.Analyze(analyzeObject.GetType());
            }
            else
            {
                // 2. If the object is null, we can't call GetType(). 
                // <T> (Generics) are used instead of 'object' to correctly handle 'null' inputs.
                // Using 'object obj' and calling 'obj.GetType()' would throw a NullReferenceException
                // if the user passed in 'null'.
                // <T> avoids this by using 'typeof(T)', which gets the STATIC (compile-time)
                // type of the variable, which is known even if the value is 'null'.
                Type type = typeof(T);
                this.Analyze(type);
            }

        }

        /// <summary>
        /// Starts a new analysis for the given <see cref="Type"/>.
        /// </summary>
        /// <param name="analyzeType">The <see cref="Type"/> to analyze. Pass 'null' to reset the analyst to the <see cref="TypeCategory.Null"/> state.</param>
        public void Analyze(Type analyzeType)
        {
            // reset all properties
            this.UnderlyingTypes = new List<Type>();
            this.TypeCategory = TypeCategory.Null;
            // Setting AnalyzeType triggers RefreshTypeDataCollection(),
            // which then updates TypKind and UnderlyingTypes.
            this.AnalyzeType = analyzeType;
        }
        #endregion

        #region Helper Functions
        /// <summary>
        /// The internal analysis engine. This method is triggered by the <see cref="AnalyzeType"/> property setter.
        /// It categorizes the <see cref="AnalyzeType"/> and populates the <see cref="UnderlyingTypes"/> list.
        /// </summary>
        private void RefreshTypeDataCollection()
        {
            if (this.AnalyzeType != null)
            {
                // Nullable<T>: T is ONLY usable for Value Types (e.g. int, bool ...)
                // if we use Nullable.GetUnderlyingType(T) and T is something like int? (System.Nullable`1[System.Int32])
                // it will return T or in this example int (System.Int32)
                // otherwise it will be null
                Type? underlyingType = Nullable.GetUnderlyingType(this.AnalyzeType);

                if (IsSimpleDataType(this.AnalyzeType))
                {
                    this.TypeCategory = TypeCategory.Simple;
                    this.UnderlyingTypes.Add(AnalyzeType);
                }
                else if (underlyingType != null && IsSimpleDataType(underlyingType))
                {
                    this.TypeCategory = TypeCategory.SimpleNullable;
                    this.UnderlyingTypes.Add(underlyingType);
                }
                else if (this.AnalyzeType.IsArray)
                {
                    Type? elementTyp = this.AnalyzeType.GetElementType();
                    if (elementTyp != null)
                    {
                        this.TypeCategory = TypeCategory.Array;
                        this.UnderlyingTypes.Add(elementTyp);
                    }
                    else
                    {
                        this.TypeCategory = TypeCategory.Error;
                    }
                }
                else if (this.AnalyzeType.IsGenericType)
                {
                    // old logic : Type genericTypeDefinition = this.AnalyzeType.GetGenericTypeDefinition();
                    // if(genericTypeDefinition == typeof(List<>) && else if (genericTypeDefinition == typeof(Dictionary<,>))

                    // Get the generic arguments (e.g. List<string> -> 0. string || Dictionary<string, int> -> 0. string 1. int
                    Type[] genericArguments = this.AnalyzeType.GetGenericArguments();

                    // Get all implemented interfaces
                    Type[] interfaces = this.AnalyzeType.GetInterfaces();

                    // Default value for a generic type
                    this.TypeCategory = TypeCategory.OtherGenericType;

                    foreach (Type interfaceType in interfaces)
                    {
                        // e.g. List<> (generic) has implemented more than iList<>
                        // IList (old List, "not normal List") is not generic, IList<> is generic
                        // so we have to check if the interface is generic

                        // interfaceType e.g. List<string> but we whant to compare Interface<>(List<>) with List<>
                        // and not Intercae<string> (List<string) > with List<> because its not the same

                        // with GetGenericTypeDefinition we seperate List<> from List<string>
                        if (interfaceType.IsGenericType)
                        {
                            Type genericDef = interfaceType.GetGenericTypeDefinition();
                            if (genericDef == typeof(IDictionary<,>))
                            {
                                this.TypeCategory = TypeCategory.IDictionary;
                                break;
                            }
                            else if (genericDef == typeof(IList<>))
                            {
                                // Not breaking the function because IDictionary and IList could be implemented, and priority is given to IDictionary.
                                this.TypeCategory = TypeCategory.IList;
                            }
                        }
                    }
                    this.UnderlyingTypes.AddRange(genericArguments);
                }
                else
                {
                    this.TypeCategory = TypeCategory.None;
                }
            }
        }

        #region simple categoriser function

        /// <summary>
        /// Gets a value indicating whether the analyzed type is a simple data type.
        /// </summary>
        /// <returns><c>true</c> if the <see cref="TypeCategory"/> is <see cref="TypeCategory.Simple"/>; otherwise, <c>false</c>.</returns>
        public bool IsSimple()
        {
            return this.TypeCategory == TypeCategory.Simple;
        }

        /// <summary>
        /// Gets a value indicating whether the analyzed type is a simple nullable data type.
        /// </summary>
        /// <returns><c>true</c> if the <see cref="TypeCategory"/> is <see cref="TypeCategory.SimpleNullable"/>; otherwise, <c>false</c>.</returns>
        public bool IsSimpleNullable()
        {
            return this.TypeCategory == TypeCategory.SimpleNullable;
        }

        /// <summary>
        /// Gets a value indicating whether the analyzed type is either a simple type or a simple nullable type.
        /// </summary>
        /// <returns><c>true</c> if the type is simple or simple nullable; otherwise, <c>false</c>.</returns>
        public bool IsSimpleOrSimpleNullable()
        {
            return IsSimple() || IsSimpleNullable();
        }

        /// <summary>
        /// Checks if the given <see cref="Type"/> is considered a simple, non-collection data type.
        /// <br/>This includes:
        /// <br/>- All <see cref="Type.IsPrimitive"/> types (Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, Single)
        /// <br/>- <see cref="string"/>
        /// <br/>- <see cref="decimal"/>
        /// <br/>- <see cref="DateTime"/>
        /// <br/>- <see cref="DateTimeOffset"/>
        /// <br/>- <see cref="TimeSpan"/>
        /// <br/>- All <see cref="Type.IsEnum"/> types
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to check.</param>
        /// <returns>True if the type is simple; otherwise, false.</returns>
        public static bool IsSimpleDataType(Type type)
        {
            bool result = false;
            // IsPrimitive : Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64,
            // IntPtr, UIntPtr, Char, Double, Single (float)
            if (type != null)
            {
                if (type.IsPrimitive ||
                type == typeof(string) ||
                type == typeof(decimal) ||
                type == typeof(DateTime) ||
                type == typeof(DateTimeOffset) ||
                type == typeof(TimeSpan) ||
                type.IsEnum)
                {
                    result = true;
                }
            }

            return result;
        }

        /// 
        /// <summary>
        /// Checks if the given <see cref="Type"/> is a simple nullable value type.
        /// <br/>This is true only if the type is a <see cref="System.Nullable{T}"/> AND
        /// its underlying type (T) matches the criteria of <see cref="IsSimpleDataType"/>.
        /// <br/>This includes:
        /// <br/>- All <see cref="Type.IsPrimitive"/> types (Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, Single)
        /// <br/>- <see cref="string"/>
        /// <br/>- <see cref="decimal"/>
        /// <br/>- <see cref="DateTime"/>
        /// <br/>- <see cref="DateTimeOffset"/>
        /// <br/>- <see cref="TimeSpan"/>
        /// <br/>- All <see cref="Type.IsEnum"/> types
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to check.</param>
        /// <returns>True if the type is a simple nullable; otherwise, false.</returns>
        public static bool IsSimpleNullableType(Type type)
        {
            bool result = false;
            if (type != null)
            {
                // Nullable<T>: T is ONLY usable for Value Types (e.g. int, bool ...)
                // if we use Nullable.GetUnderlyingType(T) and T is something like int? (System.Nullable`1[System.Int32])
                // it will return T or in this example int (System.Int32)
                // otherwise it will be null
                Type? underlyingType = Nullable.GetUnderlyingType(type);
                if (underlyingType != null && IsSimpleDataType(underlyingType))
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Checks if the given <see cref="Type"/> is either a simple type OR a simple nullable type.
        /// <br/>It combines the results of <see cref="IsSimpleDataType"/> and <see cref="IsSimpleNullableType"/>.
        /// <br/>
        /// <br/>This will return <c>true</c> for types such as:
        /// <br/>- <c>int</c>, <c>string</c>, <c>DateTime</c> (from <see cref="IsSimpleDataType"/>)
        /// <br/>- <c>int?</c>, <c>bool?</c>, <c>DateTime?</c> (from <see cref="IsSimpleNullableType"/>)
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to check.</param>
        /// <returns>True if the type is simple or simple nullable; otherwise, false.</returns>
        public static bool IsSimpleNullableTypeOrSimpleType(Type type)
        {
            bool result = false;
            if (type != null)
            {
                if (IsSimpleDataType(type) || IsSimpleNullableType(type))
                {
                    result = true;
                }
            }
            return result;
        }
        #endregion

        public override string ToString()
        {
            string result = string.Empty;

            result += $"Analyze Type Category: {this.TypeCategory}\n";
            switch (TypeCategory)
            {
                case TypeCategory.Simple:
                    result += $"AnalyzeType: {this.AnalyzeType}\n";
                    result += $"UnderlyingType: {this.UnderlyingTypes[0]}\n";
                    break;

                case TypeCategory.SimpleNullable:
                    result += $"AnalyzeType: {this.AnalyzeType}\n";
                    result += $"UnderlyingType: {this.UnderlyingTypes[0]}\n";
                    break;

                case TypeCategory.Array:
                case TypeCategory.IList:
                    result += $"AnalyzeType: {this.AnalyzeType}\n";
                    result += $"UnderlyingType: {this.UnderlyingTypes[0]}\n";
                    break;

                case TypeCategory.IDictionary:
                    result += $"AnalyzeType: {this.AnalyzeType}\n";
                    result += $"UnderlyingType Key: {this.UnderlyingTypes[0]}\n";
                    result += $"UnderlyingType Value: {this.UnderlyingTypes[1]}\n";
                    break;

                case TypeCategory.OtherGenericType:
                    result += $"AnalyzeType: {this.AnalyzeType}\n";
                    result += $"UnderlyingType: {this.UnderlyingTypes[0]}\n";
                    break;

                case TypeCategory.Null:
                case TypeCategory.Error:
                default:
                    break;
            }
            //result += "\n";
            return result;
        }
        #endregion
    }
}