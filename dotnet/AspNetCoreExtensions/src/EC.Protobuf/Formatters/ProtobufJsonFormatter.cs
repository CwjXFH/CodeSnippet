using System.Reflection;
using EC.Protobuf.Exceptions;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace EC.Protobuf.Formatters;

/// <summary>
/// Convert <see cref="IMessage"/> to JSON format
/// </summary>
public static class ProtobufJsonFormatter
{
    private static JsonFormatter _jsonFormatter = JsonFormatter.Default;
    private static JsonParser _jsonParser = JsonParser.Default;
    private static readonly Dictionary<string, MessageDescriptor> TypeMsgDescriptorMap = new();

    public static JsonFormatter JsonFormatter => _jsonFormatter;
    public static JsonParser JsonParser => _jsonParser;


    internal static IReadOnlyDictionary<string, MessageDescriptor> TypeMessageDescriptorMap => TypeMsgDescriptorMap;

    /// <summary>
    /// Register <see cref="MessageDescriptor"/> into <see cref="JsonFormatter"/> ，used to convert <see cref="IMessage"/> to JSON format
    /// </summary>
    /// <exception cref="ApplicationException">Cannot get the entry point in the assembly</exception>
    internal static void RegistryMessageDescriptor()
    {
        var entryAssembly = Assembly.GetEntryAssembly() ?? throw new NotFoundEntryAssemblyException("Cannot get entry assembly");
        var referencedAssemblies = entryAssembly.GetReferencedAssemblies();

        var assemblies = new List<AssemblyName> { entryAssembly.GetName() };
        assemblies.AddRange(referencedAssemblies);

        var allDescriptorList = new List<MessageDescriptor>();
        foreach (var assemblyName in assemblies)
        {
            var assembly = Assembly.Load(assemblyName);
            var descriptorList = RegistryMessageDescriptorCore(assembly);
            allDescriptorList.AddRange(descriptorList);
        }

        var typeRegistry = TypeRegistry.FromMessages(allDescriptorList);
        _jsonFormatter = new JsonFormatter(new JsonFormatter.Settings(true, typeRegistry).WithFormatEnumsAsIntegers(true));
        _jsonParser = new JsonParser(JsonParser.Settings.Default.WithTypeRegistry(typeRegistry).WithIgnoreUnknownFields(true));
    }


    private static IReadOnlyList<MessageDescriptor> RegistryMessageDescriptorCore(Assembly assembly)
    {
        var messageTypes = assembly.GetTypes()
            .Where(t => t.IsAbstract == false)
            .Where(t => t.IsAssignableTo(typeof(IMessage)));

        var descriptorList = new List<MessageDescriptor>();
        foreach (var msgType in messageTypes)
        {
            var descriptorProperty = msgType.GetProperty("Descriptor");
            if (descriptorProperty == null)
            {
                continue;
            }

            if (descriptorProperty.GetValue(msgType) is MessageDescriptor messageDescriptor)
            {
                descriptorList.Add(messageDescriptor);
                TypeMsgDescriptorMap[msgType.FullName!] = messageDescriptor;
            }
        }

        return descriptorList;
    }
}
