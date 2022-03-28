import { observer } from 'mobx-react-lite';
import * as React from 'react';
import { Button, Card, Grid, Header, Image, Tab } from 'semantic-ui-react';
import { Profile } from '../../app/models/profile';
import { useStore } from '../../app/stores/store';


interface Props {
    profile: Profile | null;
}

export default observer(function ProfilePhotos({ profile }: Props) {
    const { profileStore: { isCurrentUser } } = useStore();
    const [addPhotoMode, setAddPhotoMode] = React.useState(false);

    return (
        <Tab.Pane>
            <Grid>
                <Grid.Column width={16}>
                    <Header floated='left' icon='image' content='Photos' />
                    {isCurrentUser && (
                        <Button floated='right' basic content={addPhotoMode ? 'Cancel' : 'Add Photo'}
                            onClick={() => setAddPhotoMode(!addPhotoMode)}
                        />

                    )}
                </Grid.Column>
                <Grid.Column width={16}>
                    {addPhotoMode ? (
                        <p>Phot Widget goes here</p>
                    ) : (
                        <Card.Group itemsPerRow={5}>
                            {profile?.photos?.map(photo => (
                                <Card key={photo.id}>
                                    <Image src={photo.url || '/assets/user.png'} />
                                </Card>
                            ))}
                        </Card.Group>
                    )}
                </Grid.Column>
            </Grid>

        </Tab.Pane>
    );
})