﻿// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function GetAllUsers() {
    $.ajax({
        url: "/Home/GetAllUsers",
        method: "GET",
        success: function (data) {
            let content = "";
            for (var i = 0; i < data.length; i++) {
                let style = '';
                let subContent = '';
                if (data[i].hasRequestPending) {
                    subContent = `<button class='btn btn-outline-secondary' onclick="TakeRequest('${data[i].id}')">Already Sent</button>`
                }
                else {
                    subContent = `<div class='btn btn-outline-primary' onclick="SendFollow('${data[i].id}')" >Follow</div>`;
                }
                if (data[i].isOnline) {
                    style = "border:5px solid springgreen";
                }
                else {
                    style = "border:5px solid red";
                }
                const item = `
                    <div class='card' style='${style};width:220px:margin:5px'>

                    <img style='width:100%;height:220px' src='/images/${data[i].image}' />
                    <div class='card-body'>
                        <h5 class='card-title'>${data[i].userName}</h5>
                        <p class='card-text'>${data[i].email}</p>
                    </div>

                    ${subContent}

                    </div>
                `;

                content += item;
            }

            $("#allUsers").html(content);

        }
    })
}

function GetMyRequests() {
    $.ajax({
        url: "/Home/GetAllRequests",
        method: "GET",
        success: function (data) {
            $("#requests").html("");
            let content = "";
            let subContent = "";
            for (let i = 0; i < data.length; i++) {
                if (data[i].status == "Request") {
                    subContent = `
                    <div class='card-body'>
                    <button class='btn btn-success' >Accept</button>
                    <button class='btn btn-warning' >Decline</button>
                    </div>
                    `;
                }
                else {
                    subContent = `
                    <div class='card-body'>
                    <button class='btn btn-warning' >Delete</button>
                    </div>
                    `;
                }

                let item = `
                    <div class='card' style='width:15rem;'>
                    <div class='card-body'>
                        <h5>Request</h5>
                        <ul class='list-group list-group-flush'>
                            <li>${data[i].content}</li>
                        </ul>
                        ${subContent}
                    </div>
                    </div>
                `;

                content += item;

            }

            $("#requests").html(content);
        }
    })
}

GetMyRequests();
GetAllUsers();


function SendFollow(id) {
    $.ajax({
        url: `/Home/SendFollow/${id}`,
        method: "GET",
        success: function (data) {
            const element = document.querySelector("#alert");
            element.style.display = "block";
            element.innerHTML = "Your friend request sent successfully";
            SendFollowCall(id);
            GetAllUsers();

            setTimeout(() => {
                element.innerHTML = "";
                element.style.display = "none";
            }, 5000);
        }
    })
}

function TakeRequest(id) {
    $.ajax({
        url: `/Home/TakeRequest/${id}`,
        method: "DELETE",
        success: function (data) {
            const element = document.querySelector("#alert");
            element.style.display = "block";
            element.innerHTML = "You has taken your request successfully successfully";
            SendFollowCall(id);
            GetAllUsers();

            setTimeout(() => {
                element.innerHTML = "";
                element.style.display = "none";
            }, 5000);
        }
    })
}