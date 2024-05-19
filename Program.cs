// See https://aka.ms/new-console-template for more information

// constants
const int LINE_NUMBER_OFFSET = 1;
const string TAB_CHARS = "    ";
const int LINE_START = 6;

// global variables
var charsIn = new List<List<char>> { new List<char>() };
int cursorPositionVertical = 0;
int cursorPositionHorizontal = 6;

// console settings
Console.TreatControlCAsInput = true;


void DrawLineNumbers(int offset)
{
    int currHeight = Console.WindowHeight;
    for (int i = 0; i < currHeight; i++)
    {
        Console.SetCursorPosition(0, i);
        int adjustedLineNumber = i + offset;
        Console.BackgroundColor = ConsoleColor.Blue;
        int multiplier = 5 - adjustedLineNumber.ToString().Length;
        string lineNumber = new string('0', multiplier);
        Console.Write(lineNumber + adjustedLineNumber);
    }
    Console.ResetColor();
}

void DrawInfoBar()
{
    Console.SetCursorPosition(0, Console.WindowHeight);
    Console.BackgroundColor = ConsoleColor.Blue;
    string infoLine = $"      Line: {cursorPositionVertical + LINE_NUMBER_OFFSET} Column: {cursorPositionHorizontal - LINE_START + 1}";
    string filler = new string(' ', Console.WindowWidth - infoLine.Length);
    Console.Write(infoLine + filler);
    Console.ResetColor();
    Console.SetCursorPosition(cursorPositionVertical, cursorPositionHorizontal);
}

void SaveFile()
{
    string? file_name = null;
    while (file_name == null)
    {
        Console.Clear();
        Console.Write("enter the name of the file to be saved: ");
        file_name = Console.ReadLine();
    }
    using (var outputFile = new StreamWriter(file_name))
    {
        Console.WriteLine($"writing to file: {file_name}");
        for (int i = 0; i < charsIn.Count; i++)
        {
            List<char> line = charsIn[i];
            outputFile.WriteLine(new string(line.ToArray()));
        }
    }
}

void OpenFile()
{
    string? file_name = null;
    while (file_name == null)
    {
        Console.Clear();
        Console.Write("enter the name of the file to be opened: ");
        file_name = Console.ReadLine();
    }
    using (var inputFile = new StreamReader(file_name))
    {
        Console.WriteLine($"opening file: {file_name}");
        charsIn = new List<List<char>>();
        string? current_line = inputFile.ReadLine();
        while (current_line is not null)
        {
            var line = new List<char>([.. current_line]);
            charsIn.Add(line);
            current_line = inputFile.ReadLine();
        }
    }
}

void ExitProgram()
{
    Console.Clear();
    Environment.Exit(0);
}

void HandleInput()
{
    ConsoleKeyInfo input = Console.ReadKey();
    if (input.Modifiers == ConsoleModifiers.Control && input.Key == ConsoleKey.S)
    {
        SaveFile();
    }
    if (input.Modifiers == ConsoleModifiers.Control && input.Key == ConsoleKey.O)
    {
        OpenFile();
    }
    if (input.Modifiers == ConsoleModifiers.Control && input.Key == ConsoleKey.C)
    {
        ExitProgram();
    }
    switch (input.Key)
    {
        case ConsoleKey.Backspace:
            if (charsIn[cursorPositionVertical].Count - 1 >= 0)
            {
                charsIn[cursorPositionVertical].RemoveAt(charsIn[cursorPositionVertical].Count - 1);
                cursorPositionHorizontal--;
            }
            else if (charsIn[cursorPositionVertical].Count - 1 < 0 && cursorPositionVertical > 0)
            {
                cursorPositionVertical--;
                cursorPositionHorizontal = charsIn[cursorPositionVertical].Count + 6;
            }
            break;
        case ConsoleKey.Enter:
            cursorPositionVertical++;
            cursorPositionHorizontal = 6;
            charsIn.Add(new List<char>());
            break;
        case ConsoleKey.DownArrow:
            if (cursorPositionVertical + 1 < charsIn.Count)
            {
                cursorPositionVertical++;
                cursorPositionHorizontal = charsIn[cursorPositionVertical].Count + 6;
            }
            break;
        case ConsoleKey.UpArrow:
            if (cursorPositionVertical - 1 >= 0)
            {
                cursorPositionVertical--;
                cursorPositionHorizontal = charsIn[cursorPositionVertical].Count + 6;
            }
            break;
        case ConsoleKey.LeftArrow:
            if (cursorPositionHorizontal - 1 >= 0)
            {
                cursorPositionHorizontal--;
            }
            break;
        case ConsoleKey.RightArrow:
            if (cursorPositionHorizontal + 1 < charsIn[cursorPositionVertical].Count)
            {
                cursorPositionHorizontal++;
            }
            break;
        case ConsoleKey.Tab:
            foreach (char tab_char in TAB_CHARS)
            {
                charsIn[cursorPositionVertical].Add(tab_char);
                cursorPositionHorizontal++;
            }
            break;
        default:
            charsIn[cursorPositionVertical].Add(input.KeyChar);
            cursorPositionHorizontal++;
            break;
    }
}

void DrawLine(int cursorVertical, List<char> line)
{
    Console.SetCursorPosition(LINE_START, cursorVertical);
    int horizontalOffset = line.Count - (Console.WindowWidth - LINE_START);
    if (horizontalOffset > 0)
    {
        Console.Write(line.ToArray()[horizontalOffset..^2]);
        Console.BackgroundColor = ConsoleColor.Blue;
        Console.Write("|>");
        Console.ResetColor();
    }
    else
    {
        Console.Write(line.ToArray());
    }
}

void DrawText()
{
    for (int i = 0; i < charsIn.Count; i++)
    {
        DrawLine(i, charsIn[i]);
    }
    Console.SetCursorPosition(cursorPositionHorizontal, cursorPositionVertical);
}

void Main()
{
    Console.Clear();
    DrawLineNumbers(LINE_NUMBER_OFFSET);
    DrawInfoBar();
    DrawText();
    HandleInput();
}

while (true)
{
    Main();
}

