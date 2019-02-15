/*
create database ImageTagging;

use ImageTagging;

create table Image (
ID		int		not null	primary key,
ImageFilePath varchar(max) not null,
Picture	varchar(max) not null,
Description varchar(1000) null
)

create table Tag (
PictureID	int		not null	foreign key references Image(ID) on update cascade on delete cascade,
Tag			varchar(255)	not null
)

insert into Image(ID, Picture, Description) values (1, 'http://www.thinkstockphotos.com/ts-resources/images/home/TS_AnonHP_462882495_01.jpg', null);
insert into Tag(PictureID, Tag) values (1, 'Turtle');
insert into Tag(PictureID, Tag) values (1, 'Nature');
insert into Tag(PictureID, Tag) values (1, 'Sea');
insert into Tag(PictureID, Tag) values (1, 'Water');
insert into Tag(PictureID, Tag) values (1, 'Coral');

*/

/*
select I.ID, I.ImageFilePath, I.Picture, I.Description, T.Tag
from Image I inner join Tag T on I.ID = T.PictureID
order by I.ID asc

select I.ID, I.Picture, T.Tag
from Image I inner join Tag T on I.ID = T.PictureID
order by I.ID asc

select * 
from Image;

delete from Tag;
delete from Image;

drop table Tag;
drop table Image;
*/

