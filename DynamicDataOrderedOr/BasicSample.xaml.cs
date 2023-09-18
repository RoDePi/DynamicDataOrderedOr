using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using DynamicData;
using DynamicData.Binding;

namespace DynamicDataOrderedOr;

public partial class BasicSample : IDisposable
{
    #region Static Fields and Constants
    private static readonly char[] Chars =
    {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
        'w', 'x', 'y', 'z'
    };

    private static readonly Random Random = new();
    #endregion

    #region Fields
    private readonly CompositeDisposable _cleanUp;
    private readonly SourceList<string> _sourceList1;
    private readonly SourceList<string> _sourceList2;
    #endregion

    #region Properties and Indexers
    public ReadOnlyObservableCollection<string> SortedList1 { get; }
    public ReadOnlyObservableCollection<string> SortedList2 { get; }
    public ReadOnlyObservableCollection<string> SortedListsCombined1 { get; }
    public ReadOnlyObservableCollection<string> SortedListsCombined2 { get; }
    public ReadOnlyObservableCollection<string> SortedListsMerged { get; }
    public ReadOnlyObservableCollection<string> SortedListsTransformMany { get; }
    #endregion

    #region Constructors
    public BasicSample()
    {
        _cleanUp = new CompositeDisposable();

        _sourceList1 = new SourceList<string>();
        _cleanUp.Add(_sourceList1);
        _cleanUp.Add(
            _sourceList1
                .Connect()
                .Sort(SortExpressionComparer<string>.Ascending(s => s))
                .Bind(out var sortedList1)
                .Subscribe());
        SortedList1 = sortedList1;

        _sourceList2 = new SourceList<string>();
        _cleanUp.Add(_sourceList2);
        _cleanUp.Add(
            _sourceList2
                .Connect()
                .Sort(SortExpressionComparer<string>.Descending(s => s))
                .Bind(out var sortedList2)
                .Subscribe());
        SortedList2 = sortedList2;

        _sourceList1.AddRange(new[]
        {
            RandomString(), RandomString(), RandomString(), RandomString(), RandomString()
        });
        _sourceList2.AddRange(new[]
        {
            RandomString(), RandomString(), RandomString(), RandomString(), RandomString()
        });

        _cleanUp.Add(
            _sourceList1
                .Connect()
                .Sort(SortExpressionComparer<string>.Ascending(s => s))
                .Or(_sourceList2
                    .Connect()
                    .Sort(SortExpressionComparer<string>.Descending(s => s)))
                .Bind(out var sortedListsCombined1)
                .Subscribe());
        SortedListsCombined1 = sortedListsCombined1;

        var orList = new SourceList<IObservable<IChangeSet<string>>>();
        orList.Add(_sourceList1
            .Connect()
            .Sort(SortExpressionComparer<string>.Ascending(s => s)));
        orList.Add(_sourceList2
            .Connect()
            .Sort(SortExpressionComparer<string>.Descending(s => s)));
        _cleanUp.Add(orList.Or().Bind(out var sortedListsCombined2).Subscribe());
        SortedListsCombined2 = sortedListsCombined2;

        _cleanUp.Add(_sourceList1
            .Connect()
            .Sort(SortExpressionComparer<string>.Ascending(s => s))
            .Merge(_sourceList2
                .Connect()
                .Sort(SortExpressionComparer<string>.Descending(s => s)))
            .Bind(out var sortedListsMerged).Subscribe());
        SortedListsMerged = sortedListsMerged;

        var sourceLists = new SourceList<IObservableList<string>>();
        sourceLists.AddRange(new[]
        {
            _sourceList1.Connect().Sort(SortExpressionComparer<string>.Ascending(s => s)).AsObservableList(),
            _sourceList2.Connect().Sort(SortExpressionComparer<string>.Descending(s => s)).AsObservableList()
        });
        _cleanUp.Add(sourceLists
            .Connect()
            .TransformMany(sl => sl)
            .Bind(out var sortedListsTransformMany)
            .Subscribe());
        SortedListsTransformMany = sortedListsTransformMany;

        InitializeComponent();
    }
    #endregion

    #region Interface Implementations
    public void Dispose()
    {
        _cleanUp.Dispose();
    }
    #endregion

    #region Private members
    private void AddToList1(object sender, RoutedEventArgs e)
    {
        _sourceList1.Add(RandomString());
    }

    private void AddToList2(object sender, RoutedEventArgs e)
    {
        _sourceList2.Add(RandomString());
    }

    private static string RandomString(int length = 5)
    {
        var stringBuilder = new StringBuilder();
        for (var i = 0; i < length; i++)
        {
            stringBuilder.Append(Chars[Random.Next(Chars.Length)]);
        }
        return stringBuilder.ToString();
    }
    #endregion
}
