// Bin2Hex.cpp : Определяет точку входа для приложения.
//

#include "framework.h"
#include "Bin2Hex.h"
#include "stdafx.h"
#include "HexView.h"
#include <string>

#define MAX_LOADSTRING 100
//#define APPNAME                 _T("Bin2Hex")

// Глобальные переменные:
HINSTANCE hInst;                                // текущий экземпляр
WCHAR szTitle[MAX_LOADSTRING];                  // Текст строки заголовка
WCHAR szWindowClass[MAX_LOADSTRING];            // имя класса главного окна
WCHAR szPOZ[100]; //Текст для статистики
HWND hStaticText;//текстовый контрол
HWND hInfo;//текстовая ночальная информация
HWND            hMain;//главный контрол окна
HWND            hHexView;//контрол отображения файла
///////////Выбор файла
HANDLE          hFileMap;
ULONG_PTR       FileBase;
HANDLE          hFile;//Окно открытия файлов
///////////
OPTIONS         Options;
WCHAR           FilePath[MAX_PATH];//имя файла
LARGE_INTEGER   FileSize;//Размер файла

// Отправить объявления функций, включенных в этот модуль кода:
ATOM                MyRegisterClass(HINSTANCE hInstance);
BOOL                InitInstance(HINSTANCE, int);
LRESULT CALLBACK    WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK    About(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK    Review(HWND, UINT, WPARAM, LPARAM);


INT isReview = 0;
INT ReviewCount = 0;
VOID FileOpen();
VOID FileClose();
VOID SetWindowTitleText(_In_z_ PCTSTR Text);
VOID LoadSettingsRegistry();
VOID SaveSettingsRegistry();

int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPWSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);

    // TODO: Разместите код здесь.

    // Инициализация глобальных строк
    LoadStringW(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
    LoadStringW(hInstance, IDC_BIN2HEX, szWindowClass, MAX_LOADSTRING);
    LoadString(hInstance, TEX_POZ, szPOZ, 100);
    MyRegisterClass(hInstance);
 

  
    // Выполнить инициализацию приложения:
    if (!InitInstance (hInstance, nCmdShow))
    {
        MessageBox(0, _T("CreateWindowEx failed!"), szWindowClass, MB_ICONERROR);
        return FALSE;
    }
    
    HACCEL hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_BIN2HEX));

    MSG msg;

    // Цикл основного сообщения:
    while (GetMessage(&msg, nullptr, 0, 0))
    {
        if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg))
        {
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }
    }

    return (int) msg.wParam;
}



//
//  ФУНКЦИЯ: MyRegisterClass()
//
//  ЦЕЛЬ: Регистрирует класс окна.
//
ATOM MyRegisterClass(HINSTANCE hInstance)
{
    WNDCLASSEXW wcex;

    wcex.cbSize = sizeof(WNDCLASSEX);

    //wcex.style          = CS_HREDRAW | CS_VREDRAW;
    wcex.style = 0;
    wcex.lpfnWndProc    = WndProc;
    wcex.cbClsExtra     = 0;
    wcex.cbWndExtra     = 0;
    wcex.hInstance      = hInstance;
    wcex.hIcon          = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_BIN2HEX));
    wcex.hCursor        = LoadCursor(nullptr, IDC_ARROW);
    wcex.hbrBackground  = (HBRUSH)(COLOR_WINDOW+1);
    wcex.lpszMenuName   = MAKEINTRESOURCEW(IDC_BIN2HEX);
    wcex.lpszClassName  = szWindowClass;
    wcex.hIconSm        = LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

    return RegisterClassExW(&wcex);
}

//
//   ФУНКЦИЯ: InitInstance(HINSTANCE, int)
//
//   ЦЕЛЬ: Сохраняет маркер экземпляра и создает главное окно
//
//   КОММЕНТАРИИ:
//
//        В этой функции маркер экземпляра сохраняется в глобальной переменной, а также
//        создается и выводится главное окно программы.
//
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
   hInst = hInstance; // Сохранить маркер экземпляра в глобальной переменной
   /*hMain = CreateWindowEx(WS_EX_ACCEPTFILES,
       szWindowClass,
       szWindowClass,
       WS_OVERLAPPEDWINDOW | WS_CLIPCHILDREN,
       Options.WindowPlacement.rcNormalPosition.left,
       Options.WindowPlacement.rcNormalPosition.top,
       Options.WindowPlacement.rcNormalPosition.right,
       Options.WindowPlacement.rcNormalPosition.bottom,
       0,
       0,
       hInstance,
       NULL);*/
   hMain = CreateWindowW(szWindowClass,
       szTitle, 
       WS_OVERLAPPEDWINDOW,
      CW_USEDEFAULT,
       0, 
       CW_USEDEFAULT, 
       0, 
       nullptr, 
       nullptr, 
       hInstance, 
       nullptr);

   if (!hMain)
   {
      return FALSE;
   }
   DragAcceptFiles(hMain, TRUE);
   ShowWindow(hMain, nCmdShow);
   UpdateWindow(hMain);

   return TRUE;
}

//
//  ФУНКЦИЯ: WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  ЦЕЛЬ: Обрабатывает сообщения в главном окне.
//
//  WM_COMMAND  - обработать меню приложения
//  WM_PAINT    - Отрисовка главного окна
//  WM_DESTROY  - отправить сообщение о выходе и вернуться
//
//
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        RECT rc;
    case WM_CREATE:
    {
        RegisterClassHexView();

        hHexView = CreateHexView(hWnd);


        //CreateWindow(L"BUTTON", L"Нажми меня", WS_VISIBLE | WS_CHILD | BS_DEFPUSHBUTTON,
          //  10, 10, 120, 40, hWnd, (HMENU)BUTTON_ID, NULL, NULL);
        hStaticText = CreateWindow(
            L"STATIC",                      // Класс окна для статического текста
            // L"Позиция:",   // Текст
            szPOZ,
            WS_CHILD | WS_VISIBLE | SS_LEFT, // Стили: дочернее, видимое, выравнивание слева
            0, 0, 0, 0,                // X, Y, Ширина, Высота
            hWnd,                    // Дескриптор родительского окна
            NULL,                          // ID элемента (можно задать свой)
            NULL,                         // Дескриптор приложения
            NULL                           // Дополнительные данные
        );
        WCHAR inf[100];
        LoadString(hInst, IDS_Info, inf, 100);
     
        hInfo = CreateWindow(
            L"STATIC",                      // Класс окна для статического текста
            // L"Позиция:",   // Текст
            inf,
            WS_CHILD | WS_VISIBLE  | SS_CENTER, // Стили: дочернее, видимое, выравнивание слева
            0, 0, 350, 200,                // X, Y, Ширина, Высота
            hWnd,                    // Дескриптор родительского окна
            NULL,                          // ID элемента (можно задать свой)
            NULL,                         // Дескриптор приложения
            NULL                           // Дополнительные данные
        );
       // InvalidateRect(hInfo, NULL, TRUE); // Стереть и перерисовать
       // UpdateWindow(hInfo);

       


        break;
    }
    case WM_NOTIFY:
    {
        LPNMHDR NmHdr = (LPNMHDR)lParam;

        if (NmHdr->hwndFrom == hHexView) {

            if (NmHdr->code == HVN_GETDISPINFO) {

                PNMHVDISPINFO DispInfo = (PNMHVDISPINFO)lParam;

                if (DispInfo->Item.Mask & HVIF_ADDRESS) {

                    DispInfo->Item.Address = DispInfo->Item.NumberOfItem;
                }
                else if (DispInfo->Item.Mask & HVIF_BYTE) {

                    PBYTE Base = (PBYTE)(FileBase + DispInfo->Item.NumberOfItem);

                    DispInfo->Item.Value = *Base;

                    //
                    // Set state of the item.
                    //

                    if (DispInfo->Item.NumberOfItem >= 0 && DispInfo->Item.NumberOfItem <= 255) {

                        DispInfo->Item.State = HVIS_MODIFIED;
                    }
                }
            }
            if (NmHdr->code == HVN_ITEMCHANGING) {

                PNMHEXVIEW HexView = (PNMHEXVIEW)lParam;

                PBYTE Base = (PBYTE)(FileBase + HexView->Item.NumberOfItem);

                *Base = HexView->Item.Value;
            }
            if (NmHdr->code == HVN_StaticText)
            {
                PNMHVDISPINFO DispInfo = (PNMHVDISPINFO)lParam;
                //std::wstring s = " " + std::to_wstring(DispInfo->Item.NumberOfItem);
                std::wstring poz = szPOZ;// L"Позиция ";
                poz.append(L": ");
                poz.append(std::to_wstring(DispInfo->Item.NumberOfItem));



                SendMessage(hStaticText, WM_SETTEXT, 0, (LPARAM)(poz.c_str()));

                //  DestroyWindow(hStaticText);
            }
        }

        break;
    }
    case WM_DROPFILES:
    {
        if (DragQueryFile((HDROP)wParam, 0, FilePath, _countof(FilePath))) {

            FileOpen();
        }

        break;
    }
    case WM_SETFOCUS:

        SetFocus(hHexView);
        break;

    case WM_CLOSE:
       // DialogBox(hInst, MAKEINTRESOURCE(IDD_ReviewBOX), hMain, Review);
        FileClose();

        //SaveSettings();
        LoadSettingsRegistry();
        if (ReviewCount > 5 && isReview == 0)
        {
            DialogBox(hInst, MAKEINTRESOURCE(IDD_ReviewBOX), hMain, Review);
        }
        else
        {
            if (isReview == 0)
            {


                ReviewCount++;
                SaveSettingsRegistry();
            }
        }
        
        DestroyWindow(hWnd);
        break;
    case WM_SIZE:
    {
        GetClientRect(hWnd, &rc);

        MoveWindow(hHexView,
            rc.left,
            rc.top,
            rc.right - rc.left,
            rc.bottom - rc.top - 35,
            TRUE);

        MoveWindow(hStaticText,
            10,
            rc.bottom - 25,
            rc.right - rc.left,
            25,
            TRUE);
        if (hInfo != NULL) {
            int parentWidth = LOWORD(lParam);  // Новая ширина родителя
            int parentHeight = HIWORD(lParam); // Новая высота родителя

            int staticWidth = 550;
            int staticHeight = 200;

            // Вычисляем новые координаты (X, Y)
            int x = (parentWidth - staticWidth) / 2;
            int y = (parentHeight - staticHeight) / 2;

            // Перемещаем элемент
            MoveWindow(hInfo, x, y, staticWidth, staticHeight, TRUE);
        }
        //InvalidateRect(hInfo, NULL, TRUE); // Стереть и перерисовать
       //UpdateWindow(hInfo);
        break;
    }
    case WM_COMMAND:
        {
            int wmId = LOWORD(wParam);
            // Разобрать выбор в меню:
            switch (wmId)
            {
            case IDM_ABOUT:
                DialogBox(hInst, MAKEINTRESOURCE(IDD_ABOUTBOX), hWnd, About);
                break;
            case IDM_RUS:
                DialogBox(hInst, MAKEINTRESOURCE(IDD_RUSBOX), hWnd, About);
                break;
            case IDM_EXIT:
                DestroyWindow(hWnd);
                break;
            case IDM_FILE_OPEN:
            {
                
                OPENFILENAME ofn = { 0 };

                ofn.lStructSize = sizeof(OPENFILENAME);
                ofn.hwndOwner = hWnd;
                ofn.hInstance = GetModuleHandle(NULL);
                ofn.lpstrFilter = _T("All Files(*.*)\0*.*\0Executable Files(*.exe)\0*.exe\0Dynamic Link Library(*.dll)\0*.dll\0System Files(*.sys)\0*.sys\0\0");
                ofn.lpstrFile = FilePath;
                ofn.nMaxFile = _countof(FilePath);
                ofn.lpstrTitle = _T("Open File");
                ofn.Flags = OFN_EXPLORER | OFN_FILEMUSTEXIST | OFN_HIDEREADONLY;

                if (GetOpenFileName(&ofn)) 
                {

                    FileOpen();
                }
                DestroyWindow(hInfo);
                break;
            }
            case IDM_FILE_CLOSE:

                FileClose();
                GetClientRect(hWnd, &rc);
                WCHAR inf[100];
                LoadString(hInst, IDS_Info, inf, 100);

                if (hInfo != NULL) {
                    int parentWidth = rc.right;  // Новая ширина родителя
                    int parentHeight = rc.bottom; // Новая высота родителя

                    int staticWidth = 550;
                    int staticHeight = 200;

                    // Вычисляем новые координаты (X, Y)
                    int x = (parentWidth - staticWidth) / 2;
                    int y = (parentHeight - staticHeight) / 2;
                    hInfo = CreateWindow(
                        L"STATIC",                      // Класс окна для статического текста
                        // L"Позиция:",   // Текст
                        inf,
                        WS_CHILD | WS_VISIBLE | SS_CENTER, // Стили: дочернее, видимое, выравнивание слева
                        //rc.right / 2 - rc2.right/2,
                        x,
                        y,
                        staticWidth,
                        staticHeight,                 // X, Y, Ширина, Высота
                        hWnd,                    // Дескриптор родительского окна
                        NULL,                          // ID элемента (можно задать свой)
                        NULL,                         // Дескриптор приложения
                        NULL                           // Дополнительные данные
                    );
                    // Перемещаем элемент
                    MoveWindow(hInfo, x, y, staticWidth, staticHeight, TRUE);
                }
              
                
                break;
            default:
                return DefWindowProc(hWnd, message, wParam, lParam);
            }
        }
        break;
    case WM_PAINT:
        {
            PAINTSTRUCT ps;
            HDC hdc = BeginPaint(hWnd, &ps);
            // TODO: Добавьте сюда любой код прорисовки, использующий HDC...

            EndPaint(hWnd, &ps);
        }
        break;
    case WM_DESTROY:
        PostQuitMessage(0);
        break;
    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }
    return 0;
}

// Обработчик сообщений для окна "О программе".
INT_PTR CALLBACK About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    UNREFERENCED_PARAMETER(lParam);
    switch (message)
    {
    case WM_INITDIALOG:
        return (INT_PTR)TRUE;

    case WM_COMMAND:
        if (LOWORD(wParam) == IDOK || LOWORD(wParam) == IDCANCEL)
        {
            EndDialog(hDlg, LOWORD(wParam));
            return (INT_PTR)TRUE;
        }
        break;
    }
    return (INT_PTR)FALSE;
}
// Обработчик сообщений для окна "Оставить отзы".
INT_PTR CALLBACK Review(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    UNREFERENCED_PARAMETER(lParam);
    switch (message)
    {
    case WM_INITDIALOG:
    {
        HWND hwndOwner;
        RECT rcDlg, rcOwner;

        // Определяем владельца окна (родителя). Если родителя нет, центрируем по экрану
        hwndOwner = GetParent(hDlg);
        if (hwndOwner == NULL) {
            hwndOwner = GetDesktopWindow();
        }

        // Получаем размеры диалога и родителя
        GetWindowRect(hDlg, &rcDlg);
        GetWindowRect(hwndOwner, &rcOwner);

        // Вычисляем координаты центра
        int nWidth = rcDlg.right - rcDlg.left;
        int nHeight = rcDlg.bottom - rcDlg.top;

        int nX = rcOwner.left + (rcOwner.right - rcOwner.left - nWidth) / 2;
        int nY = rcOwner.top + (rcOwner.bottom - rcOwner.top - nHeight) / 2;

        // Перемещаем окно
        SetWindowPos(hDlg, HWND_TOP, nX, nY, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
        return (INT_PTR)TRUE;
    }
    case WM_COMMAND:
        if (LOWORD(wParam) == IDOK || LOWORD(wParam) == IDCANCEL)
        {
            EndDialog(hDlg, LOWORD(wParam));
            ReviewCount = 0;
            SaveSettingsRegistry();
            return (INT_PTR)TRUE;
        }
        if (LOWORD(wParam) == IDREV)
        {
            std::wstring uri = L"ms-windows-store://review/?ProductId=9NNHSR22R4NF";
           

            // Open the URI via the default system handler (Microsoft Store)
            ShellExecuteW(NULL, L"open", uri.c_str(), NULL, NULL, SW_SHOWNORMAL);
            EndDialog(hDlg, LOWORD(wParam));
            isReview = 1;
            SaveSettingsRegistry();
            return (INT_PTR)TRUE;
        }
        break;
    }

    return (INT_PTR)FALSE;
}
VOID FileOpen()
{
    FileClose();

    hFile = CreateFile(FilePath,GENERIC_READ,FILE_SHARE_READ,0,OPEN_EXISTING,FILE_ATTRIBUTE_NORMAL,0);

    if (INVALID_HANDLE_VALUE != hFile) {

        GetFileSizeEx(hFile, &FileSize);

        hFileMap = CreateFileMapping(hFile, NULL, PAGE_READONLY, 0, 0, NULL);

        if (hFileMap) {

            FileBase = (ULONG_PTR)MapViewOfFile(hFileMap, FILE_MAP_READ, 0, 0, 0);

            if (FileBase) {

                DWORD Style = 0;

                Style |= FileSize.HighPart ? HVS_ADDRESS64 : 0;
                Style |= HVS_READONLY;

                SendMessage(hHexView, HVM_SETEXTENDEDSTYLE, 0, Style);
                SendMessage(hHexView, HVM_SETITEMCOUNT, 0, (LPARAM)FileSize.LowPart);

                SetWindowTitleText(FilePath);
                std::wstring poz = szPOZ;// L"Позиция ";
                poz.append(L": 0");
            



                SendMessage(hStaticText, WM_SETTEXT, 0, (LPARAM)(poz.c_str()));
              
            }
            else 
            {
                DialogBox(hInst, MAKEINTRESOURCE(IDD_ERRORBOX), hMain, About);
                CloseHandle(hFileMap);
                CloseHandle(hFile);
            }
        }
        else {
            DialogBox(hInst, MAKEINTRESOURCE(IDD_ERRORBOX), hMain, About);
            CloseHandle(hFile);
        }
    }
}
VOID FileClose()
{
    if (FileBase && hFileMap && hFile) {

        SendMessage(hHexView, HVM_SETITEMCOUNT, 0, 0);

        UnmapViewOfFile((LPCVOID)FileBase);
        FileBase = NULL;

        CloseHandle(hFileMap);
        hFileMap = 0;

        CloseHandle(hFile);
        hFile = 0;

        SetWindowText(hMain, szWindowClass);

        SetFocus(hMain);
       
    }
}

VOID SetWindowTitleText(_In_z_ PCTSTR Text)
{
    TCHAR Title[MAX_PATH * 2];

    StringCchPrintf(Title,
        _countof(Title),
        _T("%s %s"),
        szWindowClass,
        Text);

    SetWindowText(hMain, Title);
}

void SaveSettingsRegistry() {
    HKEY hKey;
    // Открываем или создаем ветку настроек для приложения
    LONG result = RegCreateKeyExW(HKEY_CURRENT_USER, L"Software\\YuKO\\Bin2Hex",
        0, NULL, REG_OPTION_NON_VOLATILE, KEY_WRITE,
        NULL, &hKey, NULL);

    if (result == ERROR_SUCCESS) {
        int windowWidth = 1024;
        int windowHeight = 1024;
        int pozHeight = 1024;
        int pozWidth = 1024;
       

        // Сохраняем целочисленные значения
       // RegSetValueExW(hKey, L"WindowWidth", 0, REG_DWORD, (BYTE*)&windowWidth, sizeof(windowWidth));
       // RegSetValueExW(hKey, L"ThemeID", 0, REG_DWORD, (BYTE*)&themeId, sizeof(themeId));
        RegSetValueExW(hKey, L"isReview", 0, REG_DWORD, (BYTE*)&isReview, sizeof(isReview));
        RegSetValueExW(hKey, L"isCount", 0, REG_DWORD, (BYTE*)&ReviewCount, sizeof(ReviewCount));
        // Сохраняем строку
        const wchar_t* userName = L"JohnDoe";
        RegSetValueExW(hKey, L"UserName", 0, REG_SZ, (BYTE*)userName, (wcslen(userName) + 1) * sizeof(wchar_t));

        RegCloseKey(hKey);
       // std::wcout << L"Настройки успешно сохранены в реестр.\n";
    }
}

void LoadSettingsRegistry() {
    HKEY hKey;
    LONG result = RegOpenKeyExW(HKEY_CURRENT_USER, L"Software\\YuKO\\Bin2Hex",
        0, KEY_READ, &hKey);

    if (result == ERROR_SUCCESS) {
        DWORD value, bufferSize;

        // Чтение WindowWidth
        bufferSize = sizeof(value);
        RegQueryValueExW(hKey, L"isReview", NULL, NULL, (BYTE*)&value, &bufferSize);
        isReview = value;
        //std::wcout << L"WindowWidth: " << value << L"\n";

        // Чтение ThemeID
        bufferSize = sizeof(value);
        RegQueryValueExW(hKey, L"isCount", NULL, NULL, (BYTE*)&value, &bufferSize);
        ReviewCount = value;
        //std::wcout << L"ThemeID: " << value << L"\n";

        // Чтение строки
       // wchar_t userName[256];
      //  bufferSize = sizeof(userName);
        //RegQueryValueExW(hKey, L"UserName", NULL, NULL, (BYTE*)userName, &bufferSize);
       // std::wcout << L"UserName: " << userName << L"\n";

        RegCloseKey(hKey);
    }
    else {
       // std::wcout << L"Настройки не найдены. Используются значения по умолчанию.\n";
    }
}

