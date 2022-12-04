#include <iostream>   
#include <regex>      
#include <stack>
#include <string>
#include <map>
#include <vector>

using namespace std;

class StringFix {
    string f;                  
    vector< string> str2ary( string);  // 文字列の配列化

public:
    StringFix(string f) : f(f) {}  
    string convert();              
};
string StringFix::convert() {
    stack<string> st;  // スタック
    vector<string> v;  // 変換後配列
    regex re_d{ R"(\w+)" };     // 生文字列リテラル数値・アルファベット
    regex re_k_0("\\(");    // 文字列リテラル括弧・開き
    regex re_k_1("\\)");    // 文字列リテラル括弧・閉じ
    map< string, int> kPri = {
    {"*", 3}, {"/", 3}, {"+", 2}, {"-", 2}, {"(", 1}, {")", 1},
    };                           // 演算子・括弧優先度設定
    smatch sm;              
       
    string rpn;             // 変換後文字列

    for (string t : str2ary(f)) {       //入力されたものを逆ポーランドへ変換
        if (regex_search(t, sm, re_d)) {
            if (t == "PI") {
                
                v.push_back("3.14");
            }
            else if (t == "e") {
                v.push_back("2.18");
            }
            else {
                v.push_back(t);
            }
                
            
        }
        else if (regex_search(t, sm, re_k_0)) {
            st.push(t);
        }
        else if (regex_search(t, sm, re_k_1)) {
            while (st.top() != "(") {
                v.push_back(st.top());
                st.pop();
            }
            st.pop();
        }
        else {
            while (!st.empty() && (kPri[st.top()] >= kPri[t])) {
                v.push_back(st.top());
                st.pop();
            }
            st.push(t);
        }
    }
    while (!st.empty()) {
        v.push_back(st.top());
        st.pop();
    }
    int sz_v = v.size();
    for (int i = 0; i < sz_v - 1; i++)
        rpn += v[i] + " ";
    rpn += v[sz_v - 1];
    return rpn;
}

class Math {
    string rpn;                                
    vector< string> str2ary( string);  // 文字列の配列化
public:
    Math(string rpn) : rpn(rpn) {}  
    double calc();                      // 計算
};

double Math::calc() {
    stack<double> st;        // スタック
    regex re_d{ R"(\w+)" };      // 数字とアルファベット
    regex re_pl{R"(\+)"};      // +
    regex re_mn{ R"(\-)" };      // -
    regex re_pr{ R"(\*)" };      // *
    regex re_dv{R"(\/)"};      // /
    
    smatch sm;               
    double l;                     
    double r;                    
    double ans;                  
        for ( string t : str2ary(rpn)) {
            if ( regex_search(t, sm, re_d)) {
                st.push(stod(t));
                continue;
            }
            r = st.top();
            st.pop();
            l = st.top();
            st.pop();
            if ( regex_search(t, sm, re_pl)) {
                st.push(l + r);
            }
            else if ( regex_search(t, sm, re_mn)) {
                st.push(l - r);
            }
            else if ( regex_search(t, sm, re_pr)) {
                st.push(l * r);
            }
            else if ( regex_search(t, sm, re_dv)) {
                st.push(l / r);
            }
        }

        ans = st.top();


    return ans;
}
       
vector< string> StringFix::str2ary( string f) 
{
    vector< string> v;
    regex re("([a-zA-Z0-9\\.]+|[()*/+\\-])");
    smatch sm;

    f.erase(remove(f.begin(), f.end(),' '), f.end());
    while ( regex_search(f, sm, re)) {
        v.push_back(sm[1].str());
        f = sm.suffix();

    } 
    return v;  // 配列化成功
}

 vector< string> Math::str2ary( string rpn) {
     vector< string> v;
     regex re_0("([=\\s]+$)");  // 正規表現（末尾の=とスペース）
     regex re_1("([a-zA-Z0-9\\.]+|[()*/+\\-])");
     string fmt = "";
     smatch sm;
        rpn =  regex_replace(rpn, re_0, fmt);
        while ( regex_search(rpn, sm, re_1)) {
            v.push_back(sm[1].str());
            rpn = sm.suffix();
        }

    return v;  // 配列化成功
}

int main(int argc, char* argv[]) {
    while(1){
    string buf;
        cout << "計算式: ";
    getline( cin, buf);
    if (buf == "EXIT") {
        break;
    }
    if (buf.empty()) return 0;
    StringFix i2r(buf);
    string A = i2r.convert();
    //cout << A << endl;
    Math r(A);
    cout << buf<<' ' << r.calc() << endl;

    }

    /*
    電卓を制作
    掛け算や割り算,e,PI,少数を使った計算ができる。逆ポーランドのやり方を調べプログラミングした。
    */
}
    