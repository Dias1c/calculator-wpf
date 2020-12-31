using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator_WPF
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {

    private List<double?> _tbNumbers = new List<double?>(2);
    private string _operation { get; set; }
    private string _oldOperation { get; set; }


    public MainWindow()
    {
      InitializeComponent();

      List<string> styles = new List<string>() { "LiteTheme", "WindowsTheme", "IphoneTheme" };
      styleBox.SelectionChanged += ThemeChange;
      styleBox.ItemsSource = styles;
      styleBox.SelectedIndex = 0;

      _tbNumbers.Add(null);
      _tbNumbers.Add(null);
    }

    public void ThemeChange(object sender, SelectionChangedEventArgs e)
    {
      string style = styleBox.SelectedItem as string;
      var uri = new Uri(style + ".xaml", UriKind.Relative);
      ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
      Application.Current.Resources.Clear();
      Application.Current.Resources.MergedDictionaries.Add(resourceDict);
    }
    private async void Error()
    {
      tbResult.IsEnabled = false;
      tbResult.Text = "Error!";
      await Task.Delay(300);

      tbResult.Text = String.Empty;
      tbResult.IsEnabled = true;
    }

    /// <summary>
    /// Выбирает операцию и решает математику
    /// </summary>
    /// <param name="operation">знак математической операции</param>
    private void DoOperation(string operation)
    {
      try
      {

        //Проверяем операцию
        if (operation != "=" && operation != "%")
          _oldOperation = operation;
        _operation = operation;

        if (_operation.Equals("="))
        {
          //Повторяем преведущую операцию 
          DoMath(_oldOperation);
        }
        else
        {
          //Делаем текущую операцию
          DoMath(_operation);
        }

      }
      catch (Exception)
      {
        Error();
      }
    }

    /// <summary>
    /// Решает математику за счет выбранной операции. Цифры берет с глобальных переменных и tbResult
    /// </summary>
    /// <param name="operation"></param>
    private void DoMath(string operation)
    {
      if (string.IsNullOrEmpty(operation))
      {
        WriteToLog("Choose operation!");
        return;
      }
      if (String.IsNullOrEmpty(tbResult.Text))
        tbResult.Text = "0";

      if (_tbNumbers[0] == null)
      {
        _tbNumbers[0] = Convert.ToDouble(tbResult.Text);
        //Решаем
        double? result = null;
        switch (operation)
        {
          case "sin(x)":
            result = Math.Round(Math.Sin(Math.PI * (double)_tbNumbers[0] / 180));
            break;

          case "cos(x)":
            result = Math.Round(Math.Cos(Math.PI * (double)_tbNumbers[0] / 180));
            break;

          case "log(x)":
            result = Math.Log10((double)_tbNumbers[0]);
            break;

          case "|x|":
            result = Math.Abs((double)_tbNumbers[0]);
            break;

          case "1/x":
            result = 1 / _tbNumbers[0];
            break;
          case "x^2":
            result = Math.Pow((double)_tbNumbers[0], 2);
            break;
          case "√x":
            result = Math.Sqrt((double)_tbNumbers[0]);
            break;

          default:
            WriteToLog("Choose operation!");
            break;
        }

        //Выводим в Log
        WriteToLog(_tbNumbers[0], operation, _tbNumbers[1], result);
        //Присваеваем число
        _tbNumbers[0] = result ?? _tbNumbers[0];
        //Чистим tbResult
        tbResult.Text = result.ToString();

        var operations = new string[] { "√x", "x^2", "1/x", "|x|", "sin(x)", "cos(x)", "log(x)" };
        if (operations.Contains(operation))
        {
          _tbNumbers[0] = null;
          _tbNumbers[1] = null;
        }

      }
      else if (_tbNumbers[1] == null)
      {
        _tbNumbers[1] = Convert.ToDouble(tbResult.Text);

        double? result = null;

        switch (operation)
        {
          //Присудствует другая кодировка символов
          case "+":
            result = _tbNumbers[0] + _tbNumbers[1];
            break;

          case "−":
            result = _tbNumbers[0] - _tbNumbers[1];
            break;

          case "÷":
            result = _tbNumbers[0] / _tbNumbers[1];
            break;

          case "×":
            result = _tbNumbers[0] * _tbNumbers[1];
            break;

          case "=":
            result = _tbNumbers[1];
            break;

          case "sin(x)":
            result = Math.Round(Math.Sin(Math.PI * (double)_tbNumbers[1] / 180));
            break;

          case "cos(x)":
            result = Math.Round(Math.Cos(Math.PI * (double)_tbNumbers[1] / 180));
            break;

          case "log(x)":
            result = Math.Log10((double)_tbNumbers[1]);
            break;

          case "|x|":
            result = Math.Abs((double)_tbNumbers[1]);
            break;
          case "1/x":
            result = 1 / _tbNumbers[1];
            break;
          case "x^2":
            result = Math.Pow((double)_tbNumbers[1], 2);
            break;
          case "√x":
            result = Math.Sqrt((double)_tbNumbers[1]);
            break;
          case "%":
            tbResult.Text = $"{_tbNumbers[0] / _tbNumbers[1]}";
            _tbNumbers[1] = null;
            DoOperation(_oldOperation);
            return;
            break;

          default:
            Error();
            break;
        }

        // Для того чтобы коректно выводилсь информация
        var operations = new string[] { "√x", "x^2", "1/x", "|x|", "sin(x)", "cos(x)", "log(x)" };
        if (operations.Contains(operation))
        {
          _tbNumbers[0] = null;
          _tbNumbers[1] = null;
          DoMath(operation);
          return;
        }

        WriteToLog(_tbNumbers[0], operation, _tbNumbers[1], result);

        _tbNumbers[0] = null;
        _tbNumbers[1] = null;

        tbResult.Text = result.ToString();
      }
    }

    /// <summary>
    /// Выводит текст в lLog.Content
    /// </summary>
    /// <param name="text">Текст который должен показатся</param>
    void WriteToLog(string text)
    {
      lLog.Content = $"{text}";
    }
    /// <summary>
    /// Записывает в lLog.Content операцию
    /// </summary>
    /// <param name="n1">Цифра 1</param>
    /// <param name="operation">Любой знак операции</param>
    /// <param name="n2">Цифра 2</param>
    /// <param name="result">= Результат операции</param>
    private void WriteToLog(double? n1, string operation, double? n2 = null, double? result = null)
    {
      string n1Str = n1.ToString() ?? "";
      string n2Str = n2.ToString() ?? "";
      string resultStr = result.ToString() ?? "";

      lLog.Content = $"{n1Str}({operation}){n2Str}" + (string.IsNullOrEmpty(resultStr) ? "" : $"={resultStr}");
    }

    private void fAddToTb(Button bNumber)
    {
      tbResult.Text = tbResult.Text + bNumber.Content;
    }

    private void Mi_Program_Close_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }
    private void Mi_Program_About_Click(object sender, RoutedEventArgs e)
    {
      Window about = new AboutProgramWindow();
      about.Show();
    }
    private void Mi_Type_Usual_Click(object sender, RoutedEventArgs e)
    {
      grid_CalcEngineering.Visibility = Visibility.Collapsed;
      grid_CalcUsual.Visibility = Visibility.Visible;
    }

    private void Mi_Type_Engineering_Click(object sender, RoutedEventArgs e)
    {
      grid_CalcEngineering.Visibility = Visibility.Visible;
      grid_CalcUsual.Visibility = Visibility.Collapsed;
    }

    private void BtnUsual_NumberPoint_Click(object sender, RoutedEventArgs e)
    {
      if (String.IsNullOrEmpty(tbResult.Text)) tbResult.Text = "0";

      tbResult.Text = tbResult.Text.Replace(",", "");

      fAddToTb(btnUsual_NumberPoint);
    }

    private void BtnUsual_NumberInversion_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        double number = Convert.ToDouble(tbResult.Text);
        number *= -1;
        tbResult.Text = number.ToString();
      }
      catch (Exception)
      {
        Error();
      }
    }

    private void BtnUsual_FuncClearAll_Click(object sender, RoutedEventArgs e)
    {
      WriteToLog("");
      _tbNumbers[0] = null;
      _tbNumbers[1] = null;
      tbResult.Text = $"{0}";
    }

    private void BtnUsual_FuncClear_Click(object sender, RoutedEventArgs e)
    {
      tbResult.Text = $"{0}";
    }

    private void BtnUsual_FuncDel_Click(object sender, RoutedEventArgs e)
    {
      string text = tbResult.Text;
      if (String.IsNullOrEmpty(text) || text.Length == 1 || (text[0].Equals('-') && text.Length == 2))
      {
        tbResult.Text = $"{0}";
        return;
      }
      else
      {
        tbResult.Text = text.Remove(text.Length - 1);
        return;
      }
    }

    private void BtnUsual_Number0_Click(object sender, RoutedEventArgs e)
    {
      fAddToTb((Button)sender);
    }
    private void BtnUsual_Number1_Click(object sender, RoutedEventArgs e)
    {
      fAddToTb((Button)sender);
    }
    private void BtnUsual_Number2_Click(object sender, RoutedEventArgs e)
    {
      fAddToTb((Button)sender);
    }
    private void BtnUsual_Number3_Click(object sender, RoutedEventArgs e)
    {
      fAddToTb((Button)sender);
    }
    private void BtnUsual_Number4_Click(object sender, RoutedEventArgs e)
    {
      fAddToTb((Button)sender);
    }
    private void BtnUsual_Number5_Click(object sender, RoutedEventArgs e)
    {
      fAddToTb((Button)sender);
    }
    private void BtnUsual_Number6_Click(object sender, RoutedEventArgs e)
    {
      fAddToTb((Button)sender);
    }
    private void BtnUsual_Number7_Click(object sender, RoutedEventArgs e)
    {
      fAddToTb((Button)sender);
    }
    private void BtnUsual_Number8_Click(object sender, RoutedEventArgs e)
    {
      fAddToTb((Button)sender);
    }
    private void BtnUsual_Number9_Click(object sender, RoutedEventArgs e)
    {
      fAddToTb((Button)sender);
    }

    private void BtnUsual_FuncSplit_Click(object sender, RoutedEventArgs e)
    {
      string operation = ((Button)sender).Content.ToString();
      DoOperation(operation);
    }

    private void BtnUsual_FuncMultiply_Click(object sender, RoutedEventArgs e)
    {
      string operation = ((Button)sender).Content.ToString();
      DoOperation(operation);
    }

    private void BtnUsual_FuncMinus_Click(object sender, RoutedEventArgs e)
    {
      string operation = ((Button)sender).Content.ToString();
      DoOperation(operation);
    }

    private void BtnUsual_FuncPlus_Click(object sender, RoutedEventArgs e)
    {
      string operation = ((Button)sender).Content.ToString();
      DoOperation(operation);
    }

    private void BtnUsual_FuncEquals_Click(object sender, RoutedEventArgs e)
    {
      string operation = ((Button)sender).Content.ToString();
      DoOperation(operation);
    }

    private void BtnUsual_Func1SplitX_Click(object sender, RoutedEventArgs e)
    {
      string operation = ((Button)sender).Content.ToString();
      DoOperation(operation);

    }

    private void BtnUsual_FuncPowX_Click(object sender, RoutedEventArgs e)
    {
      string operation = ((Button)sender).Content.ToString();
      DoOperation(operation);

    }

    private void BtnUsual_FuncSqrtX_Click(object sender, RoutedEventArgs e)
    {
      string operation = ((Button)sender).Content.ToString();
      DoOperation(operation);
    }

    private void BtnUsual_FuncPrecent_Click(object sender, RoutedEventArgs e)
    {
      string operation = ((Button)sender).Content.ToString();
      DoOperation(operation);
    }

    //CalcEngineering
    private void BtnEngineering_FuncPrecent_Click(object sender, RoutedEventArgs e)
    {
      string operation = ((Button)sender).Content.ToString();
      DoOperation(operation);
    }

    private void BtnEngineering_FuncClear_Click(object sender, RoutedEventArgs e)
    {
      tbResult.Text = $"{0}";
    }

    private void BtnEngineering_FuncClearAll_Click(object sender, RoutedEventArgs e)
    {
      WriteToLog("");
      _tbNumbers[0] = null;
      _tbNumbers[1] = null;
      tbResult.Text = $"{0}";
    }

    private void BtnEngineering_FuncDel_Click(object sender, RoutedEventArgs e)
    {
      string text = tbResult.Text;
      if (String.IsNullOrEmpty(text) || text.Length == 1 || (text[0].Equals('-') && text.Length == 2))
      {
        tbResult.Text = $"{0}";
        return;
      }
      else
      {
        tbResult.Text = text.Remove(text.Length - 1);
        return;
      }
    }

    private void BtnEngineering_FuncSin_Click(object sender, RoutedEventArgs e)
    {
      string operation = ((Button)sender).Content.ToString();
      DoOperation(operation);
    }

    private void BtnEngineering_FuncCos_Click(object sender, RoutedEventArgs e)
    {
      string operation = ((Button)sender).Content.ToString();
      DoOperation(operation);
    }

    private void BtnEngineering_FuncLog_Click(object sender, RoutedEventArgs e)
    {
      string operation = ((Button)sender).Content.ToString();
      DoOperation(operation);
    }

    private void BtnEngineering_FuncModul_Click(object sender, RoutedEventArgs e)
    {
      string operation = ((Button)sender).Content.ToString();
      DoOperation(operation);
    }

    private void BtnEngineering_Func1SplitX_Click(object sender, RoutedEventArgs e)
    {
      string operation = ((Button)sender).Content.ToString();
      DoOperation(operation);
    }

    private void BtnEngineering_FuncPowX_Click(object sender, RoutedEventArgs e)
    {
      string operation = ((Button)sender).Content.ToString();
      DoOperation(operation);
    }

    private void BtnEngineering_FuncSqrtX_Click(object sender, RoutedEventArgs e)
    {
      string operation = ((Button)sender).Content.ToString();
      DoOperation(operation);
    }

    private void BtnEngineering_FuncSplit_Click(object sender, RoutedEventArgs e)
    {
      string operation = ((Button)sender).Content.ToString();
      DoOperation(operation);
    }

    private void BtnEngineering_Number0_Click(object sender, RoutedEventArgs e)
    {
      fAddToTb((Button)sender);
    }
    private void BtnEngineering_Number1_Click(object sender, RoutedEventArgs e)
    {
      fAddToTb((Button)sender);
    }
    private void BtnEngineering_Number2_Click(object sender, RoutedEventArgs e)
    {
      fAddToTb((Button)sender);
    }
    private void BtnEngineering_Number3_Click(object sender, RoutedEventArgs e)
    {
      fAddToTb((Button)sender);
    }
    private void BtnEngineering_Number4_Click(object sender, RoutedEventArgs e)
    {
      fAddToTb((Button)sender);
    }
    private void BtnEngineering_Number5_Click(object sender, RoutedEventArgs e)
    {
      fAddToTb((Button)sender);
    }
    private void BtnEngineering_Number6_Click(object sender, RoutedEventArgs e)
    {
      fAddToTb((Button)sender);
    }
    private void BtnEngineering_Number7_Click(object sender, RoutedEventArgs e)
    {
      fAddToTb((Button)sender);
    }
    private void BtnEngineering_Number8_Click(object sender, RoutedEventArgs e)
    {
      fAddToTb((Button)sender);
    }
    private void BtnEngineering_Number9_Click(object sender, RoutedEventArgs e)
    {
      fAddToTb((Button)sender);
    }

    private void BtnEngineering_FuncMultiply_Click(object sender, RoutedEventArgs e)
    {
      string operation = ((Button)sender).Content.ToString();
      DoOperation(operation);
    }

    private void BtnEngineering_FuncMinus_Click(object sender, RoutedEventArgs e)
    {
      string operation = ((Button)sender).Content.ToString();
      DoOperation(operation);
    }

    private void BtnEngineering_FuncPlus_Click(object sender, RoutedEventArgs e)
    {
      string operation = ((Button)sender).Content.ToString();
      DoOperation(operation);
    }

    private void BtnEngineering_FuncEquals_Click(object sender, RoutedEventArgs e)
    {
      string operation = ((Button)sender).Content.ToString();
      DoOperation(operation);
    }

    private void BtnEngineering_NumberInversion_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        double number = Convert.ToDouble(tbResult.Text);
        number *= -1;
        tbResult.Text = number.ToString();
      }
      catch (Exception)
      {
        Error();
      }
    }

    private void BtnEngineering_NumberPoint_Click(object sender, RoutedEventArgs e)
    {
      if (String.IsNullOrEmpty(tbResult.Text)) tbResult.Text = "0";

      tbResult.Text = tbResult.Text.Replace(",", "");

      fAddToTb(btnUsual_NumberPoint);
    }

   
  }
}
