// Copyright (c) 2023 Netified <contact@netified.io>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.

namespace SatisfactoryBot.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal class ReflectionExtensions
    {
        /// <summary>
        /// Initialize all classes in the application if they inherit/have a specific class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>List of initialized classes</returns>
        public static List<T> InitializeClasses<T>()
        {
            List<T> InitializedClasses = new();
            // If namespace contains '+' it's a sub class. We don't want to register these
            IEnumerable<Type> classes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(T)) && t.FullName != null && !t.FullName.Contains('+'));

            foreach (Type instance in classes)
            {
                if (CreateInstance(instance, out T createdInstance))
                    InitializedClasses.Add(createdInstance);
            }

            return InitializedClasses;
        }

        /// <summary>
        /// Create a generic instance from a type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Instance">The instance you want to create</param>
        /// <param name="result">If function returns true this will output the initialized instance</param>
        /// <returns>Boolean which indicates if it was able to create the instance</returns>
        private static bool CreateInstance<T>(Type Instance, out T result)
        {
            T? created = (T?)Activator.CreateInstance(Instance);
            if (created == null)
            {
                result = Activator.CreateInstance<T>();
                return false;
            }
            else
            {
                result = created;
                return true;
            }
        }
    }
}