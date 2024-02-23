using System.Reflection;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace Protobuf.Formatters;

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
        public static  IReadOnlyDictionary<string, MessageDescriptor> TypeMessageDescriptorMap => TypeMsgDescriptorMap;

        /// <summary>
        /// Register <see cref="MessageDescriptor"/> into <see cref="JsonFormatter"/> ，used to convert <see cref="IMessage"/> to JSON format
        /// </summary>
        /// <exception cref="ApplicationException">Cannot get the entry point in the assembly</exception>
        public static void RegistryMessageDescriptor()
        {
            var entryAssembly = Assembly.GetEntryAssembly() ?? throw new ApplicationException("Cannot get entry assembly");
            var referencedAssemblies = entryAssembly.GetReferencedAssemblies();

            var allDescriptorList = new List<MessageDescriptor>();
            foreach (var assemblyName in referencedAssemblies)
            {
                var assembly = Assembly.Load(assemblyName);
                var descriptorList = RegistryMessageDescriptorCore(assembly);
                allDescriptorList.AddRange(descriptorList);
            }

            var typeRegistry = TypeRegistry.FromMessages(allDescriptorList);
            _jsonFormatter = new JsonFormatter(new JsonFormatter.Settings(true, typeRegistry).WithFormatEnumsAsIntegers(true));
            _jsonParser = new JsonParser(JsonParser.Settings.Default.WithTypeRegistry(typeRegistry).WithIgnoreUnknownFields(true));
        }


        private static IList<MessageDescriptor> RegistryMessageDescriptorCore(Assembly assembly)
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