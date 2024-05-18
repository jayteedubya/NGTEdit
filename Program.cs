// See https://aka.ms/new-console-template for more information

// constants
const int LINE_NUMBER_OFFSET = 1;
const string TAB_CHARS = "    ";

// global variables
var chars_in = new List<List<char>> {new List<char>()};
int cursorPositionVertical = 0;
int cursorPositionHorizontal = 6;

// console settings
Console.TreatControlCAsInput = true;


void DrawLineNumbers(int offset) {
    int currHeight = Console.WindowHeight;
    for (int i = 0; i <= currHeight; i++) {
        Console.SetCursorPosition(0, i);
        int adjustedLineNumber = i + offset;
        Console.BackgroundColor = ConsoleColor.Blue;
        int multiplier = 5 - adjustedLineNumber.ToString().Length;
        string lineNumber = new string('0', multiplier);
        Console.Write(lineNumber + adjustedLineNumber);
    }
    Console.ResetColor();
}

void SaveFile() {
    string? file_name = null;
    while (file_name == null) {
        Console.Clear();
        Console.Write("enter the name of the file to be saved: ");
        file_name = Console.ReadLine();
    }
    using (var outputFile = new StreamWriter(file_name))
    {
        Console.WriteLine($"writing to file: {file_name}");
        for (int i = 0; i < chars_in.Count; i++) {
            List<char> line = chars_in[i];
            outputFile.WriteLine(new string(line.ToArray()));
        }
    }
}

void OpenFile() {
    string? file_name = null;
    while (file_name == null) {
        Console.Clear();
        Console.Write("enter the name of the file to be opened: ");
        file_name = Console.ReadLine();
    }
    using (var inputFile = new StreamReader(file_name))
    {
        Console.WriteLine($"opening file: {file_name}");
        chars_in = new List<List<char>>();
        string? current_line = inputFile.ReadLine();
        while (current_line is not null) {
            var line = new List<char>([.. current_line]);
            chars_in.Add(line);
            current_line = inputFile.ReadLine();
        }
    }
}

void ExitProgram() {
    Console.Clear();
    Environment.Exit(0);
}

void HandleInput() {
    ConsoleKeyInfo input = Console.ReadKey();
    if (input.Modifiers == ConsoleModifiers.Control && input.Key == ConsoleKey.S) {
        SaveFile();
    }
    if (input.Modifiers == ConsoleModifiers.Control && input.Key == ConsoleKey.O) {
        OpenFile();
    }
    if (input.Modifiers == ConsoleModifiers.Control && input.Key == ConsoleKey.C) {
        ExitProgram();
    }
    switch (input.Key) {
        case ConsoleKey.Backspace:
            if (chars_in[cursorPositionVertical].Count -1 >= 0) {
                chars_in[cursorPositionVertical].RemoveAt(chars_in[cursorPositionVertical].Count - 1);
                cursorPositionHorizontal--;
            }
            else if (chars_in[cursorPositionVertical].Count - 1 < 0 && cursorPositionVertical > 0) {
                cursorPositionVertical--;
                cursorPositionHorizontal = chars_in[cursorPositionVertical].Count + 6;
            }
            break;
        case ConsoleKey.Enter:
            cursorPositionVertical ++;
            cursorPositionHorizontal = 6;
            chars_in.Add(new List<char>());
            break;
        case ConsoleKey.DownArrow:
            if (cursorPositionVertical + 1 < chars_in.Count) {
                cursorPositionVertical ++;
                cursorPositionHorizontal = chars_in[cursorPositionVertical].Count + 6;
            }
            break;
        case ConsoleKey.UpArrow:
            if (cursorPositionVertical - 1 >= 0) {
                cursorPositionVertical --;
                cursorPositionHorizontal = chars_in[cursorPositionVertical].Count + 6;
            }
            break;
        case ConsoleKey.LeftArrow:
            if (cursorPositionHorizontal - 1 >= 0) {
                cursorPositionHorizontal--;
            }
            break;
        case ConsoleKey.RightArrow:
            if (cursorPositionHorizontal + 1 < chars_in[cursorPositionVertical].Count) {
                cursorPositionHorizontal++;
            }
            break;
        case ConsoleKey.Tab:
            foreach (char tab_char in TAB_CHARS) {
                chars_in[cursorPositionVertical].Add(tab_char);
                cursorPositionHorizontal++;
            }
            break;
        default:
            chars_in[cursorPositionVertical].Add(input.KeyChar);
            cursorPositionHorizontal++;
            break;
    }
}

void DrawText(int cursorVertical, int cursorHorizontal) {
    int offset = chars_in.Count - Console.WindowWidth > 0 ? chars_in.Count - Console.WindowWidth : 0;
    for (int i = offset; i < chars_in.Count - offset; i++) {
        Console.SetCursorPosition(6, i);
        List<char> line = chars_in[i];
        if (line.Count > Console.WindowWidth) {
            Console.Write(new string(line[..(Console.WindowWidth - 2)].ToArray()));
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Write("|>");
            Console.ResetColor();
        }
        int cursorOffset = cursorHorizontal - Console.WindowWidth > 0 ? cursorHorizontal - Console.WindowWidth : 0;
        Console.Write(new string(line[cursorOffset..^0].ToArray()));
    }
    int newCursorHorizontal = cursorHorizontal >= Console.WindowWidth ? Console.WindowWidth : cursorHorizontal;
    Console.SetCursorPosition(newCursorHorizontal, cursorVertical);
}

void Main() {
    Console.Clear();
    DrawLineNumbers(LINE_NUMBER_OFFSET);
    DrawText(cursorPositionVertical, cursorPositionHorizontal);
    HandleInput();
}

while (true) {
    Main();
}

