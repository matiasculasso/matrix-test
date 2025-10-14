var wf = new WordFinder(new List<string>
{
    "lionztbritext",
    "lionzecatscff",
    "lionhxbridadf",
    "lionztbristsd",
    "lioniebcifsff",
    "lionzebaicats",
    "iionzebtifggg",
    "oiunzebsihhhf",
    "niinzebrijjjj",
});

var words  = wf.Find(new List<string>() { "lion", "cats", "test", "text", "cats"});

foreach (var w in words)
    Console.WriteLine(w);


var wordsWithCount  = wf.FindWithCounts(new List<string>() { "lion", "cats", "cats", "test", "text"});

foreach (var item in wordsWithCount)
{
    Console.WriteLine($"word:{item.Key} count:{item.Value.ToString()}");
}

public class WordFinder
{ 
    private const int MaxDim = 64;
    
    private readonly List<string> _matrix;
    private readonly List<string> _transposeMatrix;
    public WordFinder(IEnumerable<string> matrix)
    {
        _matrix = (matrix ?? throw new ArgumentNullException(nameof(matrix))).ToList();
        if (_matrix.Count == 0) throw new ArgumentException("Matrix cannot be empty.", nameof(matrix));
        if (_matrix.Count > MaxDim) throw new ArgumentException($"Max rows is {MaxDim}.", nameof(matrix));
        
        var m = _matrix[0].Length;
        if (m == 0) throw new ArgumentException("Rows cannot be empty.", nameof(matrix));
        if (m > MaxDim) throw new ArgumentException($"Max columns is {MaxDim}.", nameof(matrix));
        if (_matrix.Any(r => r.Length != m))
            throw new ArgumentException("All rows must have the same length.", nameof(matrix));
        
        _transposeMatrix = Transpose(_matrix);
    }
    
    public IEnumerable<string> Find(IEnumerable<string> words)
    {
        var uniqueWords = words.Distinct();

        var rankedResult = FindWithCounts(uniqueWords);
        return rankedResult.Select(kv => kv.Key);
    }

    public Dictionary<string, int> FindWithCounts(IEnumerable<string> words)
    {
        var dict = new Dictionary<string, int>();
        
        var uniqueWords = words.Distinct();

        foreach (var raw in uniqueWords)
        {
            if (string.IsNullOrEmpty(raw)) continue;

            var word = raw;
            var matches = 0;

            // row streams
            foreach (var row in _matrix)
                matches += CountMatches(row, word);

            // column streams
            foreach (var col in _transposeMatrix)
                matches += CountMatches(col, word);

            if (matches > 0)
                dict[word] = matches;
        }

        return dict
            .OrderByDescending(kvp => kvp.Value)
            .ThenBy(kvp => kvp.Key)
            .Take(10)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
    private static int CountMatches(string input, string word)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(word))
            return 0;

        var count = 0;
        var start = 0;
        var step =  word.Length;

        while (true)
        {
            var idx = input.IndexOf(word, start, StringComparison.Ordinal);
            if (idx < 0) break;

            count++;
            start = idx + step;

            if (start <= idx ) break;
        }

        return count;
    }

    private static List<string> Transpose(List<string>? input)
    {
        if (input == null || input.Count == 0)
            return new List<string>();

        var n = input.Count;
        var m = input[0].Length;

        if (input.Any(s => s.Length != m))
            throw new ArgumentException("All strings must have the same length.");

        var result = new List<string>(m);

        for (var col = 0; col < m; col++)
        {
            var colChars = new char[n];
            for (var row = 0; row < n; row++)
                colChars[row] = input[row][col];

            result.Add(new string(colChars));
        }

        return result;
    }
}