using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CustomSpace
{ 
    public static class GenericSerialization
    {
        /// <summary>
        /// Serializes an object of type T to a JSON file.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize.</typeparam>
        /// <param name="objectToSerialize">The object to be serialized to JSON.</param>
        /// <param name="path">The path to the directory where the file will be saved.</param>
        /// <param name="fileName">The name of the file to be created.</param>
        /// <remarks>
        /// This method uses the Newtonsoft.Json library to serialize the object to a JSON string.
        /// The serialized JSON string is then written to a file at the specified path and filename.
        /// If the serialization or file creation fails, an error message is logged using Debug.LogError.
        /// </remarks>
        public static void SerializeToJson<T>(T objectToSerialize, string path, string fileName)
        {
            try
            {
                string filePath = Path.Combine(path, fileName); //creates the file in the folder for the asset path.
                //now, write the data to a json file
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    //write data to json
                    string jsonData = JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented);
                    writer.Write(jsonData);
                    writer.Close();
                    //logging just in case
                    Debug.Log($"Created file at: {filePath}");
                }
            }
            //gotta have error handling
            catch (IOException e)
            {
                Debug.LogError("Error creating file: " + e.Message);
            }
        }

        /// <summary>
        /// Deserializes a JSON file to an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize.</typeparam>
        /// <param name="path">The path to the directory containing the JSON file.</param>
        /// <param name="fileName">The name of the JSON file to deserialize.</param>
        /// <param name="output">The output parameter that will contain the deserialized object, if successful.</param>
        /// <returns>True if the deserialization was successful, false otherwise.</returns>
        /// <remarks>
        /// This method attempts to read the contents of the specified JSON file.
        /// If successful, it uses the Newtonsoft.Json library to deserialize the JSON string into an object of type T.
        /// The deserialized object is then stored in the output parameter.
        /// If the file cannot be read, deserialization fails, or an exception occurs, an error message is logged using Debug.LogError 
        /// and the output parameter is set to the default value for type T. The method returns false in this case.
        /// </remarks>
        public static bool DeSerializeJsonToType<T>(string path, out T output)
        {
            T OutputData;//profile we will store the output in
            try
            {
                //read text
                string jsonData = File.ReadAllText(path);//we know this works
                Debug.Log(jsonData);
                try
                {
                    OutputData = JsonConvert.DeserializeObject<T>(jsonData)!;

                    //for debugging serialization and to confirm process
                    if (OutputData != null)
                    {
                        Debug.Log("Profile deserialized successfully!");
                    }
                    else
                    {
                        Debug.Log("Deserialization failed!");

                    }

                    //output
                    output = OutputData;
                    return true;
                }
                catch (JsonException e)
                {
                    Debug.LogError("Error deserializing profile: " + e.Message);
                    output = default;
                    return false;
                }
            }
            catch (IOException e)
            {
                Debug.LogError("Error creating profile: " + e.Message);
                output = default;
                return false;
            }
        }


        public static List<string> GetFilesInFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                return Directory.GetFiles(folderPath).ToList();
            }
            else
            {
                Console.WriteLine($"Error: Folder not found at {folderPath}");
                return new List<string>();
            }
        }

        public static string GetNextFilename(string filename, string filePath)
        {
            int counter = 1;
            string newFilename = filename + " ({0})";  // Use newFilename to avoid confusion

            while (File.Exists(Path.Combine(filePath, string.Format(newFilename, counter) + ".json")))
            {
                counter++;
                Debug.Log(newFilename);
            }

            return Path.Combine(filePath, string.Format(newFilename, counter) + ".json");
        }
    }
}