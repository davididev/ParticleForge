using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class DirectoryHelper 
{
    public static string GetRelativePath(string directoryA, string directoryB)
    {
        // Make sure both paths are absolute
        directoryA = Path.GetFullPath(directoryA);
        directoryB = Path.GetFullPath(directoryB);

        // Split the paths into directories
        string[] pathA = directoryA.Split(Path.DirectorySeparatorChar);
        string[] pathB = directoryB.Split(Path.DirectorySeparatorChar);

        // Find the common root
        int commonRootLength = 0;
        while (commonRootLength < pathA.Length && commonRootLength < pathB.Length && pathA[commonRootLength] == pathB[commonRootLength])
        {
            commonRootLength++;
        }

        // Calculate the number of ".." needed to go from directoryA to the common root
        int upCount = pathA.Length - commonRootLength;

        // Construct the relative path
        string relativePath = string.Join(Path.DirectorySeparatorChar.ToString(), Enumerable.Repeat("..", upCount).Concat(pathB.Skip(commonRootLength)));

        return relativePath;
    }

    public static string GetFileName(string path)
    {
        int lastIndex = path.LastIndexOf('\\');
        if (lastIndex <= 0)
            lastIndex = path.LastIndexOf('/');
        return path.Substring(lastIndex + 1);
    }
    public static string GetDirectoryOfFile(string FileName)
    {
        int lastIndex = FileName.LastIndexOf('\\');
        if (lastIndex <= 0)
            lastIndex = FileName.LastIndexOf('/');
        return FileName.Substring(0, lastIndex);
    }

    public static string CombineDirectories(string baseDirectory, string relativeDirectory)
    {
        // Combine the base directory and the relative directory to get the absolute path
        string absoluteDirectory = Path.Combine(baseDirectory, relativeDirectory);

        // Normalize the path to handle any ".." or "." in the path
        absoluteDirectory = Path.GetFullPath(absoluteDirectory);

        return absoluteDirectory;
    }
}
