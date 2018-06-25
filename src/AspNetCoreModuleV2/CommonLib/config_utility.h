// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#pragma once

#include <httpserv.h>
#include "ahutil.h"
#include "stringu.h"
#include "exceptions.h"

class ConfigUtility
{
    #define CS_ASPNETCORE_HANDLER_SETTINGS                   L"handlerSettings"
    #define CS_ASPNETCORE_HANDLER_VERSION                    L"handlerVersion"
    #define CS_ASPNETCORE_HANDLER_SETTINGS_NAME              L"name"
    #define CS_ASPNETCORE_HANDLER_SETTINGS_VALUE             L"value"

public:
    static
    HRESULT
    FindHandlerVersion(IAppHostElement* pElement, STRU* strHandlerVersionValue)
    {
        HRESULT hr;
        CComPtr<IAppHostElement>           pHandlerSettings = nullptr;
        CComPtr<IAppHostElementCollection> pHandlerSettingsCollection = nullptr;
        CComPtr<IAppHostElement>           pHandlerVar = nullptr;
        ENUM_INDEX                         index {};
        STRU strHandlerName;
        STRU strHandlerValue;

        RETURN_IF_FAILED(GetElementChildByName(pElement, CS_ASPNETCORE_HANDLER_SETTINGS,&pHandlerSettings));
        RETURN_IF_FAILED(pHandlerSettings->get_Collection(&pHandlerSettingsCollection));

        RETURN_IF_FAILED(hr = FindFirstElement(pHandlerSettingsCollection, &index, &pHandlerVar));

        while (hr != S_FALSE)
        {
            RETURN_IF_FAILED(GetElementStringProperty(pHandlerVar, CS_ASPNETCORE_HANDLER_SETTINGS_NAME, &strHandlerName));
            RETURN_IF_FAILED(GetElementStringProperty(pHandlerVar, CS_ASPNETCORE_HANDLER_SETTINGS_VALUE, &strHandlerValue));

            if (strHandlerName.Equals(CS_ASPNETCORE_HANDLER_VERSION, TRUE))
            {
                RETURN_IF_FAILED(strHandlerVersionValue->Copy(strHandlerValue));
                break;
            }

            strHandlerName.Reset();
            strHandlerValue.Reset();
            pHandlerVar.Release();

            RETURN_IF_FAILED(hr = FindNextElement(pHandlerSettingsCollection, &index, &pHandlerVar));
        }

        return S_OK;
    }
};
