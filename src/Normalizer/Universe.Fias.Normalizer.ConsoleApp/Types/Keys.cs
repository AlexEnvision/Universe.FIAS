using System;
using System.Runtime.InteropServices;

namespace Universe.Fias.Normalizer.ConsoleApp.Types
{
    /// <summary>
    /// Задает коды и модификаторы клавиш.
    /// </summary>
    [Flags]
    [ComVisible(true)]
    public enum Keys
    {
        /// <summary>
        ///   Битовая маска для извлечения кода клавиши из значения ключа.
        /// </summary>
        KeyCode = 65535, // 0x0000FFFF
        /// <summary>
        ///   Битовая маска для извлечения модификаторов из значения ключа.
        /// </summary>
        Modifiers = -65536, // 0xFFFF0000
        /// <summary>Нет нажатых клавиш.</summary>
        None = 0,
        /// <summary>Левой кнопки мыши.</summary>
        LButton = 1,
        /// <summary>Правой кнопкой мыши.</summary>
        RButton = 2,
        /// <summary>Клавиша "Отмена".</summary>
        Cancel = RButton | LButton, // 0x00000003
        /// <summary>Средняя кнопка мыши (мыши).</summary>
        MButton = 4,
        /// <summary>Первая кнопка мыши (пяти кнопку мыши).</summary>
        XButton1 = MButton | LButton, // 0x00000005
        /// <summary>Вторая кнопка мыши (пяти кнопку мыши).</summary>
        XButton2 = MButton | RButton, // 0x00000006
        /// <summary>Клавиша BACKSPACE.</summary>
        Back = 8,
        /// <summary>Клавиша TAB.</summary>
        Tab = Back | LButton, // 0x00000009
        /// <summary>Клавиша перевода строки.</summary>
        LineFeed = Back | RButton, // 0x0000000A
        /// <summary>Клавиша CLEAR.</summary>
        Clear = Back | MButton, // 0x0000000C
        /// <summary>Клавиша RETURN.</summary>
        Return = Clear | LButton, // 0x0000000D
        /// <summary>Клавиша ВВОД.</summary>
        Enter = Return, // 0x0000000D
        /// <summary>Клавиша SHIFT.</summary>
        ShiftKey = 16, // 0x00000010
        /// <summary>Клавиша CTRL.</summary>
        ControlKey = ShiftKey | LButton, // 0x00000011
        /// <summary>Клавиша ALT.</summary>
        Menu = ShiftKey | RButton, // 0x00000012
        /// <summary>Клавиша PAUSE.</summary>
        Pause = Menu | LButton, // 0x00000013
        /// <summary>Клавиша CAPS LOCK.</summary>
        Capital = ShiftKey | MButton, // 0x00000014
        /// <summary>Клавиша CAPS LOCK.</summary>
        CapsLock = Capital, // 0x00000014
        /// <summary>Клавиша режима "Кана" редактора метода ввода.</summary>
        KanaMode = CapsLock | LButton, // 0x00000015
        /// <summary>
        ///   Клавиша режима IME Hanguel.
        ///    (поддерживается для совместимости; используйте <see langword="HangulMode" />)
        /// </summary>
        HanguelMode = KanaMode, // 0x00000015
        /// <summary>Клавиша режима "Хангыль" редактора метода ввода.</summary>
        HangulMode = HanguelMode, // 0x00000015
        /// <summary>Клавиша режима "Джунджа" редактора метода ввода.</summary>
        JunjaMode = HangulMode | RButton, // 0x00000017
        /// <summary>Ключ, последний режим редактора метода ввода.</summary>
        FinalMode = ShiftKey | Back, // 0x00000018
        /// <summary>Клавиша режима "Ханджа" редактора метода ввода.</summary>
        HanjaMode = FinalMode | LButton, // 0x00000019
        /// <summary>Клавиша режима "Кандзи" редактора метода ввода.</summary>
        KanjiMode = HanjaMode, // 0x00000019
        /// <summary>Клавиша ESC.</summary>
        Escape = KanjiMode | RButton, // 0x0000001B
        /// <summary>Клавиша преобразования IME.</summary>
        IMEConvert = FinalMode | MButton, // 0x0000001C
        /// <summary>Клавиша без преобразования IME.</summary>
        IMENonconvert = IMEConvert | LButton, // 0x0000001D
        /// <summary>
        ///   Клавиша заменяет принятия IME <see cref="F:System.Windows.Forms.Keys.IMEAceept" />.
        /// </summary>
        IMEAccept = IMEConvert | RButton, // 0x0000001E
        /// <summary>
        ///   Клавиша принятия IME.
        ///    Является устаревшей, используйте <see cref="F:System.Windows.Forms.Keys.IMEAccept" /> вместо него.
        /// </summary>
        IMEAceept = IMEAccept, // 0x0000001E
        /// <summary>Клавиша изменение режима редактора метода ввода.</summary>
        IMEModeChange = IMEAceept | LButton, // 0x0000001F
        /// <summary>Клавиша ПРОБЕЛ.</summary>
        Space = 32, // 0x00000020
        /// <summary>Клавиша PAGE UP.</summary>
        Prior = Space | LButton, // 0x00000021
        /// <summary>Клавиша PAGE UP.</summary>
        PageUp = Prior, // 0x00000021
        /// <summary>Клавиша PAGE DOWN.</summary>
        Next = Space | RButton, // 0x00000022
        /// <summary>Клавиша PAGE DOWN.</summary>
        PageDown = Next, // 0x00000022
        /// <summary>Клавиша END.</summary>
        End = PageDown | LButton, // 0x00000023
        /// <summary>Клавиша HOME.</summary>
        Home = Space | MButton, // 0x00000024
        /// <summary>Клавиша СТРЕЛКА ВЛЕВО.</summary>
        Left = Home | LButton, // 0x00000025
        /// <summary>Клавиша СТРЕЛКА ВВЕРХ.</summary>
        Up = Home | RButton, // 0x00000026
        /// <summary>Клавиша СТРЕЛКА ВПРАВО.</summary>
        Right = Up | LButton, // 0x00000027
        /// <summary>Клавиша СТРЕЛКА ВНИЗ.</summary>
        Down = Space | Back, // 0x00000028
        /// <summary>Клавиша SELECT.</summary>
        Select = Down | LButton, // 0x00000029
        /// <summary>Клавиша PRINT.</summary>
        Print = Down | RButton, // 0x0000002A
        /// <summary>Клавиша EXECUTE.</summary>
        Execute = Print | LButton, // 0x0000002B
        /// <summary>Клавиша PRINT SCREEN.</summary>
        Snapshot = Down | MButton, // 0x0000002C
        /// <summary>Клавиша PRINT SCREEN.</summary>
        PrintScreen = Snapshot, // 0x0000002C
        /// <summary>Клавишу INS.</summary>
        Insert = PrintScreen | LButton, // 0x0000002D
        /// <summary>DEL ключ.</summary>
        Delete = PrintScreen | RButton, // 0x0000002E
        /// <summary>Клавиша HELP.</summary>
        Help = Delete | LButton, // 0x0000002F
        /// <summary>Клавиша 0.</summary>
        D0 = Space | ShiftKey, // 0x00000030
        /// <summary>Клавиша 1.</summary>
        D1 = D0 | LButton, // 0x00000031
        /// <summary>Клавиша 2.</summary>
        D2 = D0 | RButton, // 0x00000032
        /// <summary>Клавиша 3.</summary>
        D3 = D2 | LButton, // 0x00000033
        /// <summary>Клавиша 4.</summary>
        D4 = D0 | MButton, // 0x00000034
        /// <summary>Клавиша 5.</summary>
        D5 = D4 | LButton, // 0x00000035
        /// <summary>Клавиша 6.</summary>
        D6 = D4 | RButton, // 0x00000036
        /// <summary>Клавиша 7.</summary>
        D7 = D6 | LButton, // 0x00000037
        /// <summary>Клавиша 8.</summary>
        D8 = D0 | Back, // 0x00000038
        /// <summary>Клавиша 9.</summary>
        D9 = D8 | LButton, // 0x00000039
        /// <summary>Клавиша A.</summary>
        A = 65, // 0x00000041
        /// <summary>Клавиша B.</summary>
        B = 66, // 0x00000042
        /// <summary>Клавиша C.</summary>
        C = B | LButton, // 0x00000043
        /// <summary>Клавиша D.</summary>
        D = 68, // 0x00000044
        /// <summary>Клавиша E.</summary>
        E = D | LButton, // 0x00000045
        /// <summary>Клавиша F.</summary>
        F = D | RButton, // 0x00000046
        /// <summary>Клавиша G.</summary>
        G = F | LButton, // 0x00000047
        /// <summary>Клавиша H.</summary>
        H = 72, // 0x00000048
        /// <summary>Клавиша I.</summary>
        I = H | LButton, // 0x00000049
        /// <summary>Клавиша J.</summary>
        J = H | RButton, // 0x0000004A
        /// <summary>Клавиша K.</summary>
        K = J | LButton, // 0x0000004B
        /// <summary>Клавиша L.</summary>
        L = H | MButton, // 0x0000004C
        /// <summary>Клавиша M.</summary>
        M = L | LButton, // 0x0000004D
        /// <summary>Клавиша N.</summary>
        N = L | RButton, // 0x0000004E
        /// <summary>Клавиша O.</summary>
        O = N | LButton, // 0x0000004F
        /// <summary>Клавиша P.</summary>
        P = 80, // 0x00000050
        /// <summary>Клавиша Q.</summary>
        Q = P | LButton, // 0x00000051
        /// <summary>Клавиша R.</summary>
        R = P | RButton, // 0x00000052
        /// <summary>Клавиша S.</summary>
        S = R | LButton, // 0x00000053
        /// <summary>Клавиша T.</summary>
        T = P | MButton, // 0x00000054
        /// <summary>Клавиша U.</summary>
        U = T | LButton, // 0x00000055
        /// <summary>Клавиша V.</summary>
        V = T | RButton, // 0x00000056
        /// <summary>Клавиша W.</summary>
        W = V | LButton, // 0x00000057
        /// <summary>Клавиша X.</summary>
        X = P | Back, // 0x00000058
        /// <summary>Клавиша Y.</summary>
        Y = X | LButton, // 0x00000059
        /// <summary>Клавиша Z.</summary>
        Z = X | RButton, // 0x0000005A
        /// <summary>
        ///   Левая клавиша с логотипом Windows (клавиатура Microsoft Natural Keyboard).
        /// </summary>
        LWin = Z | LButton, // 0x0000005B
        /// <summary>
        ///   Правая клавиша с логотипом Windows (клавиатура Microsoft Natural Keyboard).
        /// </summary>
        RWin = X | MButton, // 0x0000005C
        /// <summary>
        ///   Клавиша приложения (клавиатура Microsoft Natural Keyboard).
        /// </summary>
        Apps = RWin | LButton, // 0x0000005D
        /// <summary>Ключ компьютера спящего режима.</summary>
        Sleep = Apps | RButton, // 0x0000005F
        /// <summary>Клавиша 0 на цифровой клавиатуре.</summary>
        NumPad0 = 96, // 0x00000060
        /// <summary>Клавиша 1 на цифровой клавиатуре.</summary>
        NumPad1 = NumPad0 | LButton, // 0x00000061
        /// <summary>Клавиша 2 на цифровой клавиатуре.</summary>
        NumPad2 = NumPad0 | RButton, // 0x00000062
        /// <summary>Клавиша 3 на цифровой клавиатуре.</summary>
        NumPad3 = NumPad2 | LButton, // 0x00000063
        /// <summary>Клавиша 4 на цифровой клавиатуре.</summary>
        NumPad4 = NumPad0 | MButton, // 0x00000064
        /// <summary>Клавиша 5 на цифровой клавиатуре.</summary>
        NumPad5 = NumPad4 | LButton, // 0x00000065
        /// <summary>Клавиша 6 на цифровой клавиатуре.</summary>
        NumPad6 = NumPad4 | RButton, // 0x00000066
        /// <summary>Клавиша 7 на цифровой клавиатуре.</summary>
        NumPad7 = NumPad6 | LButton, // 0x00000067
        /// <summary>Клавиша 8 на цифровой клавиатуре.</summary>
        NumPad8 = NumPad0 | Back, // 0x00000068
        /// <summary>Клавиша 9 на цифровой клавиатуре.</summary>
        NumPad9 = NumPad8 | LButton, // 0x00000069
        /// <summary>Клавиша умножения.</summary>
        Multiply = NumPad8 | RButton, // 0x0000006A
        /// <summary>Клавиша сложения.</summary>
        Add = Multiply | LButton, // 0x0000006B
        /// <summary>Клавиша разделителя.</summary>
        Separator = NumPad8 | MButton, // 0x0000006C
        /// <summary>Клавиша вычитания.</summary>
        Subtract = Separator | LButton, // 0x0000006D
        /// <summary>Клавиша десятичного разделителя.</summary>
        Decimal = Separator | RButton, // 0x0000006E
        /// <summary>Клавиша деления.</summary>
        Divide = Decimal | LButton, // 0x0000006F
        /// <summary>Клавиша F1.</summary>
        F1 = NumPad0 | ShiftKey, // 0x00000070
        /// <summary>Клавиша F2.</summary>
        F2 = F1 | LButton, // 0x00000071
        /// <summary>Клавиша F3.</summary>
        F3 = F1 | RButton, // 0x00000072
        /// <summary>Клавиша F4.</summary>
        F4 = F3 | LButton, // 0x00000073
        /// <summary>Клавиша F5.</summary>
        F5 = F1 | MButton, // 0x00000074
        /// <summary>Клавиша F6.</summary>
        F6 = F5 | LButton, // 0x00000075
        /// <summary>Клавиша F7.</summary>
        F7 = F5 | RButton, // 0x00000076
        /// <summary>Клавиша F8.</summary>
        F8 = F7 | LButton, // 0x00000077
        /// <summary>Клавиша F9.</summary>
        F9 = F1 | Back, // 0x00000078
        /// <summary>Клавиша F10.</summary>
        F10 = F9 | LButton, // 0x00000079
        /// <summary>Клавиша F11.</summary>
        F11 = F9 | RButton, // 0x0000007A
        /// <summary>Клавиша F12.</summary>
        F12 = F11 | LButton, // 0x0000007B
        /// <summary>Клавиша F13.</summary>
        F13 = F9 | MButton, // 0x0000007C
        /// <summary>Клавиша F14.</summary>
        F14 = F13 | LButton, // 0x0000007D
        /// <summary>Клавиша F15.</summary>
        F15 = F13 | RButton, // 0x0000007E
        /// <summary>Клавиша F16.</summary>
        F16 = F15 | LButton, // 0x0000007F
        /// <summary>Клавиша F17.</summary>
        F17 = 128, // 0x00000080
        /// <summary>Клавиша F18.</summary>
        F18 = F17 | LButton, // 0x00000081
        /// <summary>Клавиша F19.</summary>
        F19 = F17 | RButton, // 0x00000082
        /// <summary>Клавиша F20.</summary>
        F20 = F19 | LButton, // 0x00000083
        /// <summary>Клавиша F21.</summary>
        F21 = F17 | MButton, // 0x00000084
        /// <summary>Клавиша F22.</summary>
        F22 = F21 | LButton, // 0x00000085
        /// <summary>Клавиша F23.</summary>
        F23 = F21 | RButton, // 0x00000086
        /// <summary>Клавиша F24.</summary>
        F24 = F23 | LButton, // 0x00000087
        /// <summary>Клавиша NUM LOCK.</summary>
        NumLock = F17 | ShiftKey, // 0x00000090
        /// <summary>Клавиша SCROLL LOCK.</summary>
        Scroll = NumLock | LButton, // 0x00000091
        /// <summary>Левая клавиша SHIFT.</summary>
        LShiftKey = F17 | Space, // 0x000000A0
        /// <summary>Правая клавиша SHIFT.</summary>
        RShiftKey = LShiftKey | LButton, // 0x000000A1
        /// <summary>Левая клавиша CTRL.</summary>
        LControlKey = LShiftKey | RButton, // 0x000000A2
        /// <summary>Правая клавиша CTRL.</summary>
        RControlKey = LControlKey | LButton, // 0x000000A3
        /// <summary>Левая клавиша ALT.</summary>
        LMenu = LShiftKey | MButton, // 0x000000A4
        /// <summary>Правая клавиша ALT.</summary>
        RMenu = LMenu | LButton, // 0x000000A5
        /// <summary>
        ///   Клавиша возврата обозревателя (Windows 2000 или более поздней версии).
        /// </summary>
        BrowserBack = LMenu | RButton, // 0x000000A6
        /// <summary>
        ///   Ключ прямой браузера (Windows 2000 или более поздней версии).
        /// </summary>
        BrowserForward = BrowserBack | LButton, // 0x000000A7
        /// <summary>
        ///   Клавиша обновления обозревателя (Windows 2000 или более поздней версии).
        /// </summary>
        BrowserRefresh = LShiftKey | Back, // 0x000000A8
        /// <summary>
        ///   Клавиша остановки обозревателя (Windows 2000 или более поздней версии).
        /// </summary>
        BrowserStop = BrowserRefresh | LButton, // 0x000000A9
        /// <summary>
        ///   Клавиша поиска обозревателя (Windows 2000 или более поздней версии).
        /// </summary>
        BrowserSearch = BrowserRefresh | RButton, // 0x000000AA
        /// <summary>
        ///   Клавиша браузера "Избранное" (Windows 2000 или более поздней версии).
        /// </summary>
        BrowserFavorites = BrowserSearch | LButton, // 0x000000AB
        /// <summary>
        ///   Клавиша home обозревателя (Windows 2000 или более поздней версии).
        /// </summary>
        BrowserHome = BrowserRefresh | MButton, // 0x000000AC
        /// <summary>
        ///   Клавиша выключения звука тома (Windows 2000 или более поздней версии).
        /// </summary>
        VolumeMute = BrowserHome | LButton, // 0x000000AD
        /// <summary>
        ///   (Windows 2000 или более поздней версии) клавиша уменьшения громкости.
        /// </summary>
        VolumeDown = BrowserHome | RButton, // 0x000000AE
        /// <summary>
        ///   (Windows 2000 или более поздней версии) Клавиша увеличения громкости.
        /// </summary>
        VolumeUp = VolumeDown | LButton, // 0x000000AF
        /// <summary>
        ///   Перехода к следующей записи ключа (Windows 2000 или более поздней версии).
        /// </summary>
        MediaNextTrack = LShiftKey | ShiftKey, // 0x000000B0
        /// <summary>
        ///   Перехода на предыдущую запись ключ (Windows 2000 или более поздней версии).
        /// </summary>
        MediaPreviousTrack = MediaNextTrack | LButton, // 0x000000B1
        /// <summary>
        ///   Клавиша остановки мультимедиа (Windows 2000 или более поздней версии).
        /// </summary>
        MediaStop = MediaNextTrack | RButton, // 0x000000B2
        /// <summary>
        ///   Клавиша приостановки воспроизведения (Windows 2000 или более поздней версии).
        /// </summary>
        MediaPlayPause = MediaStop | LButton, // 0x000000B3
        /// <summary>
        ///   Клавиша запуска почты (Windows 2000 или более поздней версии).
        /// </summary>
        LaunchMail = MediaNextTrack | MButton, // 0x000000B4
        /// <summary>
        ///   Выберите носитель ключ (Windows 2000 или более поздней версии).
        /// </summary>
        SelectMedia = LaunchMail | LButton, // 0x000000B5
        /// <summary>
        ///   Запуск приложения один ключ (Windows 2000 или более поздней версии).
        /// </summary>
        LaunchApplication1 = LaunchMail | RButton, // 0x000000B6
        /// <summary>
        ///   Клавиша запуска приложения два (Windows 2000 или более поздней версии).
        /// </summary>
        LaunchApplication2 = LaunchApplication1 | LButton, // 0x000000B7
        /// <summary>
        ///   Клавиша OEM с запятой на стандартной клавиатуре США (Windows 2000 или более поздней версии).
        /// </summary>
        OemSemicolon = MediaStop | Back, // 0x000000BA
        /// <summary>Клавиша OEM 1.</summary>
        Oem1 = OemSemicolon, // 0x000000BA
        /// <summary>
        ///   Клавиша плюса ПВТ на клавиатуре любой страны или региона (Windows 2000 или более поздней версии).
        /// </summary>
        Oemplus = Oem1 | LButton, // 0x000000BB
        /// <summary>
        ///   Клавиша OEM с запятой на клавиатуре любой страны или региона (Windows 2000 или более поздней версии).
        /// </summary>
        Oemcomma = LaunchMail | Back, // 0x000000BC
        /// <summary>
        ///   Клавиша OEM с минусом на клавиатуре любой страны или региона (Windows 2000 или более поздней версии).
        /// </summary>
        OemMinus = Oemcomma | LButton, // 0x000000BD
        /// <summary>
        ///   Ключ OEM периода на клавиатуре любой страны или региона (Windows 2000 или более поздней версии).
        /// </summary>
        OemPeriod = Oemcomma | RButton, // 0x000000BE
        /// <summary>
        ///   Клавиша вопросительного знака ПВТ на стандартной клавиатуре США (Windows 2000 или более поздней версии).
        /// </summary>
        OemQuestion = OemPeriod | LButton, // 0x000000BF
        /// <summary>Клавиша OEM 2.</summary>
        Oem2 = OemQuestion, // 0x000000BF
        /// <summary>
        ///   Клавиша OEM тильды на стандартной клавиатуре США (Windows 2000 или более поздней версии).
        /// </summary>
        Oemtilde = 192, // 0x000000C0
        /// <summary>Клавиша OEM 3.</summary>
        Oem3 = Oemtilde, // 0x000000C0
        /// <summary>
        ///   Клавиша OEM открывающая скобка на стандартной клавиатуре США (Windows 2000 или более поздней версии).
        /// </summary>
        OemOpenBrackets = Oem3 | Escape, // 0x000000DB
        /// <summary>Клавиша OEM 4.</summary>
        Oem4 = OemOpenBrackets, // 0x000000DB
        /// <summary>
        ///   Клавиша OEM вертикальной черты на стандартной клавиатуре США (Windows 2000 или более поздней версии).
        /// </summary>
        OemPipe = Oem3 | IMEConvert, // 0x000000DC
        /// <summary>Клавиша OEM 5.</summary>
        Oem5 = OemPipe, // 0x000000DC
        /// <summary>
        ///   Клавиша OEM закрывающая квадратная скобка на стандартной клавиатуре США (Windows 2000 или более поздней версии).
        /// </summary>
        OemCloseBrackets = Oem5 | LButton, // 0x000000DD
        /// <summary>Клавиша OEM 6.</summary>
        Oem6 = OemCloseBrackets, // 0x000000DD
        /// <summary>
        ///   OEM одинарной или двойной кавычки ключа на стандартной клавиатуре США (Windows 2000 или более поздней версии).
        /// </summary>
        OemQuotes = Oem5 | RButton, // 0x000000DE
        /// <summary>Клавиша OEM 7.</summary>
        Oem7 = OemQuotes, // 0x000000DE
        /// <summary>Клавиша OEM 8.</summary>
        Oem8 = Oem7 | LButton, // 0x000000DF
        /// <summary>
        ///   Угловой скобки ПВТ или обратной косой чертой на клавиатуре RT 102 ключа (Windows 2000 или более поздней версии).
        /// </summary>
        OemBackslash = Oem3 | PageDown, // 0x000000E2
        /// <summary>Клавиша OEM 102.</summary>
        Oem102 = OemBackslash, // 0x000000E2
        /// <summary>Клавиша ОБРАБОТКИ.</summary>
        ProcessKey = Oem3 | Left, // 0x000000E5
        /// <summary>
        ///   Используется для передачи символов Юникода в виде нажатий клавиш.
        ///    Значение клавиши пакета является младшим словом значения виртуальная клавиша 32 бита, используемый для методов ввода не клавиатуры.
        /// </summary>
        Packet = ProcessKey | RButton, // 0x000000E7
        /// <summary>Клавиша ATTN.</summary>
        Attn = Oem102 | CapsLock, // 0x000000F6
        /// <summary>Клавиша CRSEL.</summary>
        Crsel = Attn | LButton, // 0x000000F7
        /// <summary>Клавиша EXSEL.</summary>
        Exsel = Oem3 | D8, // 0x000000F8
        /// <summary>Клавиша ERASE EOF.</summary>
        EraseEof = Exsel | LButton, // 0x000000F9
        /// <summary>Клавиша PLAY.</summary>
        Play = Exsel | RButton, // 0x000000FA
        /// <summary>Клавиша ZOOM.</summary>
        Zoom = Play | LButton, // 0x000000FB
        /// <summary>
        ///   Константа, зарезервированная для использования в будущем.
        /// </summary>
        NoName = Exsel | MButton, // 0x000000FC
        /// <summary>Клавиша PA1.</summary>
        Pa1 = NoName | LButton, // 0x000000FD
        /// <summary>Клавиша CLEAR.</summary>
        OemClear = NoName | RButton, // 0x000000FE
        /// <summary>Клавиша SHIFT.</summary>
        Shift = 65536, // 0x00010000
        /// <summary>Клавиша CTRL.</summary>
        Control = 131072, // 0x00020000
        /// <summary>Клавиша модификатора ALT.</summary>
        Alt = 262144, // 0x00040000
    }
}