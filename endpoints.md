# Endpoints
Base URL = http://172.18.3.153:46465/

Record id = *
\
Student id = #
\
Email = ¤

Authorize(Roles = "Admin") [Udkommenteres fra POST/PUT/DELETE request's efter test]

ClientURI = 172.153.18.3:46465/API


## AbscenceController
**POST:**
<pre>
API/Abscence/Create {
    legalAbscence
    studentId
}
</pre>

**DELETE:**
\
API/Delete/Specific/AbscenceRecord/{id} *
\
API/Delete/Students/AbscenceRecords/{id} #


## ActivityController
**GET:**
\
API/Activity/Active/Students
\
API/Activity/Inactive/Students


**POST:**
<pre>
API/Activity/Create {
    activeStatus
    studentId
}
</pre>


**DELETE:**
\
API/Activity/Delete/Specific/ActivityRecord/{id} *
\
API/Activity/Delete/Students/ActivityRecords/{id} #


## LoginController
**POST:**
<pre>
API/Login/Register {
    name
    phone
    email
    passwordHash
    clientURI
}
</pre>


<pre>
API/Login/Login {
    email
    password
}
</pre>


<pre>
API/Login/Refresh {
    accessToken
    refreshToken
}
</pre>


<pre>
API/Login/Revoke {
    accessToken
    refreshToken
}
</pre>


**PUT:**
<pre>
API/Login/Confirm/Email {
	API/Login/Confirm/Email/Token
	email,
	confirmationToken
}
</pre>


<pre>
API/Login/Alter/Password {
    email
    oldPasswordHash
    newPasswordHash
    studentId
}
</pre>


<pre>
API/Login/Alter/Role {
    admin
    studentId
}
</pre>


**DELETE:**
\
API/Login/Delete/{email} ¤


## StudentController
**GET:**
<pre>
API/Student/Get/All {
    "studentId": number,
    "name": string,
    "address": string,
    "phone": string,
}
</pre>

**POST:**
<pre>
API/Student/Create {
    name
    address
    phone
}
</pre>


**DELETE:**
\
API/Student/Delete/{id} #


## TimeRecordController
**GET:**
\
API/TimeRecord/Get/Students/TimeRecords/{id} #
\
/API/TimeRecord/Get/All/Students/Latest/TimeRecord


**POST:**
<pre>
API/TimeRecord/Create {
    nfcTag
    checkStatus
}
</pre>


**DELETE:**
\
API/TimeRecord/Delete/Specific/TimeRecord/{id} *
\
API/TimeRecord/Delete/Students/TimeRecords/{id} #
